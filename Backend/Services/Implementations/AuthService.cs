using Backend.DTOs.Auth;
using Backend.DTOs.Auth.GoogleAuth;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Backend.Configurations;
using System.Net.Http.Headers;


namespace Backend.Services.Implementations;

/// <summary>
/// Implementation of authentication service for user registration, login, and Google OAuth.
/// 
/// Handles user account creation, credential validation, JWT token generation,
/// and Google OAuth integration with refresh token management.
/// </summary>
public class AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpClientFactory httpClientFactory,
AppDbContext context, JwtSettings jwtSettings, ILogger<AuthService> logger, GoogleSettings googleSettings) : IAuthService
{
    /// Register a new user account with email and password.
    /// 
    /// Creates a new IdentityUser with the provided credentials, assigns Customer role,
    /// and returns a JWT token for immediate authentication.
    /// </summary>
    public async Task<(AuthResponseDto? response, bool succeeded, IEnumerable<IdentityError>? Errors)> RegisterUserAsync(RegisterDto model)
    {
        var correlationId = Guid.NewGuid().ToString();
        try
        {
            logger.LogDebug("Starting registration - CorrelationId: {CorrelationId}", correlationId);

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                var errorCodes = string.Join(", ", result.Errors.Select(e => e.Code));
                logger.LogWarning("Failure to create user {UserName}. Errors: {ErrorCodes}, CorrelationId: {CorrelationId}", model.UserName, errorCodes, correlationId);
                return (null, false, result.Errors);
            }

            await userManager.AddToRoleAsync(user, "Customer");

            var roles = await userManager.GetRolesAsync(user);

            var token = GenerateJwtToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles
            };
            logger.LogInformation("Registration successful. UserId: {UserId}, Roles: {RoleCount}, CorrelationId: {CorrelationId}", user.Id, roles.Count, correlationId);
            return (response, true, null);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Critical error during registration: {UserName}, CorrelationId: {CorrelationId}", model.UserName, correlationId);
            var errors = new List<IdentityError>
            {
                new()
                {
                    Code  = "RegistrationError",
                    Description = "An error occurred during registration. Please try again."
                }
            };
            return (null, false, errors);
        }
    }

    /// <summary>
    /// Authenticate a user with email and password.
    /// 
    /// Validates credentials, implements timing attack mitigation by adding random delays
    /// on failure, and returns a JWT token on success.
    /// </summary>
    public async Task<(AuthResponseDto? response, bool succeeded)> LoginUserAsync(LoginDto model)
    {
        var correlationId = Guid.NewGuid().ToString();

        try
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {

                logger.LogWarning("Login attempt with empty credentials - CorrelationId: {CorrelationId}", correlationId);

                await Task.Delay(Random.Shared.Next(100, 300));

                return (null, false);
            }


            logger.LogDebug("Starting login - CorrelationId: {CorrelationId}", correlationId);

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                logger.LogWarning("Login attempt for non-existent email - CorrelationId: {CorrelationId}", correlationId);

                await Task.Delay(Random.Shared.Next(100, 300));

                return (null, false);
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false); // No lockout for simplicity in development

            if (!result.Succeeded)
            {
                var reason = result.IsLockedOut ? "Lockout" : result.IsNotAllowed ? "NotAllowed" : "InvalidCredentials";

                logger.LogWarning("Login failed. Reason: {Reason}, UserId: {UserId}, CorrelationId: {CorrelationId}", reason, user.Id, correlationId);
                await Task.Delay(Random.Shared.Next(100, 300));

                return (null, false);
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles
            };

            logger.LogInformation("Login successful. UserId: {UserId}, Roles: {RoleCount}, CorrelationId: {CorrelationId}", user.Id, roles.Count, correlationId);

            return (response, true);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Critical error during login. CorrelationId: {CorrelationId}", correlationId);
            await Task.Delay(Random.Shared.Next(100, 300));
            return (null, false);
        }
    }

    /// <summary>
    /// Handle Google OAuth callback.
    /// 
    /// Process flow:
    /// 1. Exchange authorization code for Google tokens
    /// 2. Retrieve user information from Google
    /// 3. Create or update user account
    /// 4. Store refresh token if provided
    /// 5. Generate and return application JWT token
    /// </summary>
    public async Task<(AuthResponseDto? response, bool succeeded)> GoogleCallbackAsync(string authorizationCode)
    {
        try
        {
            // Exchange authorization code for access and refresh tokens
            var tokenResponse = await ExchangeCodeForTokensAsync(authorizationCode);

            // Retrieve user information from Google using access token
            var userInfo = await GetGoogleUserInfoAsync(tokenResponse.AccessToken);

            // Find existing user or create new one
            var user = await userManager.FindByEmailAsync(userInfo.Email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = userInfo.Email,
                    Email = userInfo.Email,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return (null, false);
                }
                await userManager.AddToRoleAsync(user, "Customer");
            }

            // Store refresh token for long-lived sessions
            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                await SaveRefreshTokenAsync(user.Id, tokenResponse.RefreshToken, tokenResponse.ExpiresIn);
            }

            // Update user claims with Google profile information
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

            // Generate JWT and return response
            var roles = await userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles, userInfo.Name, userInfo.Picture);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles
            };

            return (response, true);
        }
        catch (Exception)
        {
            return (null, false);
        }
    }

    /// <summary>
    /// Exchange Google authorization code for access and refresh tokens.
    /// 
    /// Makes HTTP request to Google OAuth token endpoint using provided credentials
    /// from environment variables.
    /// </summary>
    private async Task<GoogleTokenResponseDto> ExchangeCodeForTokensAsync(string code)
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
            throw new Exception($"Failed to exchange authorization code: {errorContent}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponseDto>();

        return tokenResponse ?? throw new Exception("Failed to deserialize token response from Google");
    }

    /// <summary>
    /// Retrieve user information from Google using access token.
    /// 
    /// Calls Google userinfo endpoint and deserializes the response.
    /// </summary>
    private async Task<GoogleJwtPayloadDto> GetGoogleUserInfoAsync(string accessToken)
    {
        var client = httpClientFactory.CreateClient("GoogleApi");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("userinfo");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to retrieve user information from Google");
        }

        var jsonString = await response.Content.ReadAsStringAsync();

        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var userInfo = System.Text.Json.JsonSerializer.Deserialize<GoogleJwtPayloadDto>(jsonString, options);

        return userInfo ??  throw new Exception("Invalid user information received from Google");;
    }

    /// <summary>
    /// Store or update Google refresh token for a user.
    /// 
    /// Creates new record or updates existing one with new token and expiration time.
    /// </summary>
    public async Task SaveRefreshTokenAsync(string userId, string refreshToken, int expiresIn)
    {
        var existingToken = await context.UserGoogleTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        if (existingToken != null)
        {
            existingToken.RefreshToken = refreshToken;
            existingToken.ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
        }
        else
        {
            var newToken = new UserGoogleToken
            {
                UserId = userId,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn),
                CreatedAt = DateTime.UtcNow
            };
            context.UserGoogleTokens.Add(newToken);
        }
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Generate JWT bearer token for authentication.
    /// 
    /// Creates a token with user claims, roles, and optional Google profile information.
    /// Token expiration is configured via JWT_EXPIRATION_MINUTES environment variable.
    /// </summary>
    private string GenerateJwtToken(IdentityUser user, IList<string> roles, string? displayName = null, string? picture = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.UserName!)
        };

        if (!string.IsNullOrEmpty(displayName))
        {
            claims.Add(new Claim("display_name", displayName));
        }
        if (!string.IsNullOrEmpty(picture))
        {
            claims.Add(new Claim("picture", picture));
        }

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



