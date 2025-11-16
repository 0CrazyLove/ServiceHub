using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations;

/// <summary>
/// Implementation of authentication service for user registration, login, and Google OAuth.
/// 
/// Handles user account creation, credential validation, JWT token generation,
/// and Google OAuth integration with refresh token management.
/// </summary>
public class AuthService(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IHttpClientFactory httpClientFactory,
    AppDbContext context) : IAuthService
{
    /// <summary>
    /// Register a new user account with email and password.
    /// 
    /// Creates a new IdentityUser with the provided credentials, assigns Customer role,
    /// and returns a JWT token for immediate authentication.
    /// </summary>
    public async Task<(AuthResponseDto? response, bool succeeded)> RegisterUserAsync(RegisterDto model)
    {
        var user = new IdentityUser
        {
            UserName = model.UserName,
            Email = model.Email
        };
        var result = await userManager.CreateAsync(user, model.Password!);
        if (!result.Succeeded) return (null, false);
        
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
        return (response, true);
    }

    /// <summary>
    /// Authenticate a user with email and password.
    /// 
    /// Validates credentials, implements timing attack mitigation by adding random delays
    /// on failure, and returns a JWT token on success.
    /// </summary>
    public async Task<(AuthResponseDto? response, bool succeeded)> LoginUserAsync(LoginDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
        {
            // Timing attack mitigation: add random delay on invalid input
            await Task.Delay(Random.Shared.Next(100, 300));
            return (null, false);
        }
        
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            // Timing attack mitigation: add random delay when user not found
            await Task.Delay(Random.Shared.Next(100, 300));
            return (null, false);
        }
        
        var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            // Timing attack mitigation: add random delay on invalid password
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
        return (response, true);
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
    private async Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code)
    {
        var client = httpClientFactory.CreateClient();

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")! },
            { "client_secret", Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")! },
            { "redirect_uri", "postmessage" },
            { "grant_type", "authorization_code" }
        });

        var response = await client.PostAsync("https://oauth2.googleapis.com/token", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to exchange authorization code: {errorContent}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();

        if (tokenResponse == null)
        {
            throw new Exception("Failed to deserialize token response from Google");
        }

        return tokenResponse;
    }

    /// <summary>
    /// Retrieve user information from Google using access token.
    /// 
    /// Calls Google userinfo endpoint and deserializes the response.
    /// </summary>
    private async Task<GoogleJwtPayload> GetGoogleUserInfoAsync(string accessToken)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        
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
        
        var userInfo = System.Text.Json.JsonSerializer.Deserialize<GoogleJwtPayload>(jsonString, options);
        
        if (userInfo == null)
        {
            throw new Exception("Invalid user information received from Google");
        }
        
        return userInfo;
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
    private static string GenerateJwtToken(IdentityUser user, IList<string> roles, string? displayName = null, string? picture = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!));
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
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES")!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// Google OAuth JWT payload model.
/// 
/// Represents the user information returned by Google's userinfo endpoint.
/// Uses JsonPropertyName attributes for proper deserialization of Google's response format.
/// </summary>
public class GoogleJwtPayload
{
    /// <summary>
    /// The unique Google user ID (subject).
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("sub")]
    public string Sub { get; set; } = string.Empty;

    /// <summary>
    /// The user's email address.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Whether the email address has been verified by Google.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    /// <summary>
    /// The user's full name.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The URL to the user's profile picture.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("picture")]
    public string? Picture { get; set; }
}

/// <summary>
/// Google OAuth token response model.
/// 
/// Represents tokens returned by Google's token endpoint.
/// Includes access token for API calls and optional refresh token for long-lived sessions.
/// </summary>
public class GoogleTokenResponse
{
    /// <summary>
    /// The access token for making authenticated requests to Google APIs.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The refresh token for obtaining new access tokens (optional).
    /// Only provided on first authorization or when offline_access is requested.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// The lifetime of the access token in seconds.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}