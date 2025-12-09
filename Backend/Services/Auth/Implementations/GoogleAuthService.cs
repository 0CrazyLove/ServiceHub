using Backend.Configurations;
using Backend.DTOs.Auth.GoogleAuth;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace Backend.Services.Auth.Implementations;

/// <summary>
/// Service responsible for Google OAuth authentication operations.
/// </summary>
public class GoogleAuthService(IHttpClientFactory httpClientFactory, GoogleSettings googleSettings,
IConfigurationManager<OpenIdConnectConfiguration> configurationManager,
ILogger<GoogleAuthService> logger, UserManager<IdentityUser> userManager) : IGoogleAuthService
{
    /// <summary>
    /// Exchanges a Google authorization code for access and refresh tokens.
    /// </summary>
    /// <param name="code">The authorization code received from Google.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A DTO containing the access token, refresh token, and expiry.</returns>
    /// <exception cref="Exception">Thrown when the token exchange fails or response is invalid.</exception>
    public async Task<GoogleTokenDto> ExchangeCodeForTokensAsync(string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(code))
        {
            logger.LogError("Authorization code is null, empty or whitespace.");
            throw new ArgumentException("Authorization code cannot be null, empty or whitespace.", nameof(code));
        }

        var client = httpClientFactory.CreateClient("GoogleToken");

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", googleSettings.ClientId },
            { "client_secret", googleSettings.ClientSecret },
            { "redirect_uri", "postmessage" },
            { "grant_type", "authorization_code" }
        });

        var response = await client.PostAsync("token", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Failed to exchange authorization code: {Error}", errorContent);
            throw new Exception($"Failed to exchange authorization code: {errorContent}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenDto>(cancellationToken);

        return tokenResponse ?? throw new Exception("Failed to deserialize token response from Google");
    }

    /// <summary>
    /// Decodes and validates a Google ID token using Google's public signing keys.
    /// </summary>
    /// <param name="idToken">The raw JWT ID token string.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The decoded payload containing user claims.</returns>
    /// <exception cref="SecurityTokenException">Thrown when token validation fails (signature, issuer, audience).</exception>
    /// <exception cref="ArgumentException">Thrown when the token is empty or null.</exception>
    public async Task<GoogleJwtPayloadDto> DecodeAndValidateIdTokenAsync(string idToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            throw new ArgumentException("Token cannot be empty", nameof(idToken));
        }

        var handler = new JwtSecurityTokenHandler();

        var discoveryDocument = await configurationManager.GetConfigurationAsync(cancellationToken);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = ["https://accounts.google.com", "accounts.google.com"],
            ValidateAudience = true,
            ValidAudience = googleSettings.ClientId,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = discoveryDocument.SigningKeys,
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };

        var result = await handler.ValidateTokenAsync(idToken, validationParameters);

        if (!result.IsValid) throw new SecurityTokenException("Invalid token", result.Exception);

        if (result.SecurityToken is not JwtSecurityToken jwt || jwt.Header.Alg is not SecurityAlgorithms.RsaSha256)
        {
            throw new SecurityTokenException("Invalid token: unsupported algorithm");
        }

        string GetRequiredClaim(string type) => jwt.Claims.FirstOrDefault(c => c.Type == type)?.Value ?? throw new SecurityTokenException($"Claim '{type}' is required");

        string? GetOptionalClaim(string type) => jwt.Claims.FirstOrDefault(c => c.Type == type)?.Value;

        var sub = GetRequiredClaim("sub");
        var email = GetRequiredClaim("email");
        var name = GetRequiredClaim("name");
        var picture = GetOptionalClaim("picture");
        var emailVerifiedStr = GetOptionalClaim("email_verified");

        //Validate both boolean expressions
        var emailVerified = bool.TryParse(emailVerifiedStr, out var isVerified) && isVerified;

        return new GoogleJwtPayloadDto
        {
            Sub = sub,
            Email = email,
            EmailVerified = emailVerified,
            Name = name,
            Picture = picture
        };
    }
    /// <summary>
    /// Retrieves an existing user or creates a new one based on Google profile data.
    /// </summary>
    /// <param name="userInfo">The user information extracted from the Google ID token.</param>
    /// <returns>The IdentityUser instance if found or created; otherwise, null.</returns>

    public async Task<IdentityUser?> FindOrCreateGoogleUserAsync(GoogleJwtPayloadDto userInfo)
    {
        ArgumentNullException.ThrowIfNull(userInfo);
        
        if (userInfo.Email is null)
        {
            logger.LogError("Google user info missing email");
            return null;
        }

        var user = await userManager.FindByEmailAsync(userInfo.Email);

        if (user is not null) return user;

        user = new()
        {
            UserName = userInfo.Email,
            Email = userInfo.Email,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user);

        if (!createResult.Succeeded)
        {
            var errorCodes = string.Join(", ", createResult.Errors.Select(e => e.Code));
            logger.LogError("Failed to create user for email {Email}. Errors: {Errors}", userInfo.Email, errorCodes);
            return null;
        }

        var roleResult = await userManager.AddToRoleAsync(user, "Customer");
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));

            await userManager.DeleteAsync(user);

            logger.LogWarning("Failed to add Customer role to user {UserId}. Errors: {Errors}", user.Id, errors);

            return null;
        }
        logger.LogInformation("Created new Google user: {UserId}, Email: {Email}", user.Id, user.Email);

        return user;
    }

    /// <summary>
    /// Updates the user's claims with the latest Google profile information.
    /// </summary>
    /// <param name="user">The user entity to update.</param>
    /// <param name="userInfo">The latest user information from Google.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateGoogleClaimsAsync(IdentityUser user, GoogleJwtPayloadDto userInfo)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(userInfo);

        var existingClaims = await userManager.GetClaimsAsync(user);

        // Define the claims we want to manage
        var targetClaims = new Dictionary<string, string>
        {
            { "google_id", userInfo.Sub ?? string.Empty },
            { "google_picture", userInfo.Picture ?? string.Empty },
            { "google_name", userInfo.Name ?? string.Empty }
        };

        var claimsToAdd = new List<Claim>();
        var claimsToRemove = new List<Claim>();

        foreach (var (type, newValue) in targetClaims)
        {
            var existingClaim = existingClaims.FirstOrDefault(c => c.Type == type);

            if (existingClaim is not null)
            {
                if (!string.Equals(existingClaim.Value, newValue, StringComparison.Ordinal))
                {
                    claimsToRemove.Add(existingClaim);
                    claimsToAdd.Add(new(type, newValue));
                }
            }
            else
            {
                claimsToAdd.Add(new(type, newValue));
            }
        }

        // Execute updates only if needed to minimize DB operations
        if (claimsToRemove.Count > 0)
        {
            await userManager.RemoveClaimsAsync(user, claimsToRemove);
        }

        if (claimsToAdd.Count > 0)
        {
            await userManager.AddClaimsAsync(user, claimsToAdd);
        }

        if (claimsToAdd.Count > 0 || claimsToRemove.Count > 0)
        {
            logger.LogDebug("Updated Google claims for user: {UserId}. Removed: {RemovedCount}, Added: {AddedCount}", user.Id, claimsToRemove.Count, claimsToAdd.Count);
        }
    }
}