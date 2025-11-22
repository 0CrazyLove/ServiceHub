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
    /// Exchange Google authorization code for access and refresh tokens.
    /// </summary>
    public async Task<GoogleTokenDto> ExchangeCodeForTokensAsync(string code)
    {
        var client = httpClientFactory.CreateClient("GoogleToken");

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", googleSettings.ClientId },
            { "client_secret", googleSettings.ClientSecret },
            { "redirect_uri", "postmessage" },
            { "grant_type", "authorization_code" }
        });

        var response = await client.PostAsync("token", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to exchange authorization code: {Error}", errorContent);
            throw new Exception($"Failed to exchange authorization code: {errorContent}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenDto>();

        return tokenResponse ?? throw new Exception("Failed to deserialize token response from Google");
    }

    /// <summary>
    /// Decode and validate Google ID token using Google's public keys.
    /// </summary>
    public async Task<GoogleJwtPayloadDto> DecodeAndValidateIdTokenAsync(string idToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            throw new ArgumentException("Token cannot be empty", nameof(idToken));
        }

        var handler = new JwtSecurityTokenHandler();
        var discoveryDocument = await configurationManager.GetConfigurationAsync(CancellationToken.None);

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

        var principal = handler.ValidateToken(idToken, validationParameters, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt || jwt.Header.Alg != SecurityAlgorithms.RsaSha256)
        {
            throw new SecurityTokenException("Invalid token: unsupported algorithm");
        }

        var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? throw new SecurityTokenException("Claim 'sub' is required");
        var email = principal.FindFirstValue(JwtRegisteredClaimNames.Email) ?? throw new SecurityTokenException("Claim 'email' is required");
        var emailVerified = bool.TryParse(principal.FindFirstValue("email_verified"), out var verified) && verified;
        var name = principal.FindFirstValue(JwtRegisteredClaimNames.Name) ?? throw new SecurityTokenException("Claim 'name' is required");
        var picture = principal.FindFirstValue("picture");

        return new GoogleJwtPayloadDto
        {
            Sub = sub,
            Email = email,
            EmailVerified = emailVerified,
            Name = name,
            Picture = picture
        };
    }

    public async Task<(IdentityUser? user, bool succeeded)> FindOrCreateGoogleUserAsync(GoogleJwtPayloadDto userInfo)
    {
        var user = await userManager.FindByEmailAsync(userInfo.Email);

        if (user != null)
        {
            return (user, true);
        }

        // Create new user
        user = new IdentityUser
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
            return (null, false);
        }

        await userManager.AddToRoleAsync(user, "Customer");
        logger.LogInformation("Created new Google user: {UserId}, Email: {Email}", user.Id, user.Email);

        return (user, true);
    }

    /// <summary>
    /// Update user's Google-related claims (google_id, google_picture, google_name).
    /// </summary>
    public async Task UpdateGoogleClaimsAsync(IdentityUser user, GoogleJwtPayloadDto userInfo)
    {
        var existingClaims = await userManager.GetClaimsAsync(user);
        var googleClaims = existingClaims.Where(c =>
            c.Type == "google_id" ||
            c.Type == "google_picture" ||
            c.Type == "google_name").ToList();

        if (googleClaims.Any())
        {
            await userManager.RemoveClaimsAsync(user, googleClaims);
        }

        var claims = new List<Claim>
        {
            new("google_id", userInfo.Sub),
            new("google_picture", userInfo.Picture ?? ""),
            new("google_name", userInfo.Name ?? "")
        };

        await userManager.AddClaimsAsync(user, claims);
        logger.LogDebug("Updated Google claims for user: {UserId}", user.Id);
    }
}