using Backend.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Backend.Services.Auth.Interfaces;
using Backend.Configurations;

namespace Backend.Services.Auth.Implementations;

/// <summary>
/// Orchestrator service for authentication operations.
/// Coordinates between specialized services to handle user registration, login, and Google OAuth.
/// </summary>
public class AuthService(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IJwtTokenService jwtTokenService, JwtSettings jwtSettings,
    IGoogleAuthService googleAuthService,
    IRefreshTokenService refreshTokenService,
    ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor) : IAuthService
{

    /// <inheritdoc />
    public async Task<(AuthResponseDto? response, bool succeeded)> RegisterUserAsync(RegisterDto model)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;

        try
        {
            logger.LogDebug("Starting registration - CorrelationId: {CorrelationId}", correlationId);

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password!);

            if (!result.Succeeded)
            {
                var errorCodes = string.Join(", ", result.Errors.Select(e => e.Code));
                logger.LogWarning("Failed to create user {UserName}. Errors: {ErrorCodes}, CorrelationId: {CorrelationId}", model.UserName, errorCodes, correlationId);
                await Task.Delay(Random.Shared.Next(100, 300));
                return (null, false);
            }

            await userManager.AddToRoleAsync(user, "Customer");

            var roles = await userManager.GetRolesAsync(user);
            var (token, refreshToken) = await GenerateAndSaveTokensAsync(user, roles);

            var response = CreateAuthResponse(token, refreshToken, user.UserName!, user.Email!, roles);

            logger.LogInformation("Registration successful. UserId: {UserId}, Roles: {RoleCount}, CorrelationId: {CorrelationId}", user.Id, roles.Count, correlationId);

            return (response, true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during registration: {UserName}, CorrelationId: {CorrelationId}", model.UserName, correlationId);
            await Task.Delay(Random.Shared.Next(100, 300));
            return (null, false);
        }
    }

    /// <inheritdoc />
    public async Task<(AuthResponseDto? response, bool succeeded)> LoginUserAsync(LoginDto model)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;

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

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                var reason = result.IsLockedOut ? "Lockout" : result.IsNotAllowed ? "NotAllowed" : "InvalidCredentials";
                logger.LogWarning("Login failed. Reason: {Reason}, UserId: {UserId}, CorrelationId: {CorrelationId}", reason, user.Id, correlationId);
                await Task.Delay(Random.Shared.Next(100, 300));
                return (null, false);
            }

            var roles = await userManager.GetRolesAsync(user);
            var (token, refreshToken) = await GenerateAndSaveTokensAsync(user, roles);

            var response = CreateAuthResponse(token, refreshToken, user.UserName!, user.Email!, roles);

            logger.LogInformation("Login successful for user {UserId} with {RoleCount} role(s). CorrelationId: {CorrelationId}", user.Id, roles.Count, correlationId);
            return (response, true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Critical error during login. CorrelationId: {CorrelationId}", correlationId);
            await Task.Delay(Random.Shared.Next(100, 300));
            return (null, false);
        }
    }

    /// <inheritdoc />
    public async Task<(AuthResponseDto? response, bool succeeded)> GoogleCallbackAsync(string authorizationCode, CancellationToken cancellationToken)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;

        logger.LogDebug("Starting Google OAuth callback. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            var tokenResponse = await googleAuthService.ExchangeCodeForTokensAsync(authorizationCode, cancellationToken);

            if (string.IsNullOrEmpty(tokenResponse.IdToken))
            {
                logger.LogWarning("Google token response missing ID token. CorrelationId: {CorrelationId}", correlationId);
                await Task.Delay(Random.Shared.Next(100, 300));
                return (null, false);
            }
            var userInfo = await googleAuthService.DecodeAndValidateIdTokenAsync(tokenResponse.IdToken, cancellationToken);

            var user = await googleAuthService.FindOrCreateGoogleUserAsync(userInfo);

            if (user is null)
            {
                logger.LogWarning("Failed to find or create Google user for email: {Email}. CorrelationId: {CorrelationId}", userInfo.Email, correlationId);
                await Task.Delay(Random.Shared.Next(100, 300));
                return (null, false);
            }

            await googleAuthService.UpdateGoogleClaimsAsync(user, userInfo);

            var roles = await userManager.GetRolesAsync(user);
            var (token, refreshToken) = await GenerateAndSaveTokensAsync(user, roles, userInfo.Name, userInfo.Picture, cancellationToken);

            var response = CreateAuthResponse(token, refreshToken, userInfo.Name!, userInfo.Email!, roles);

            logger.LogInformation("Google OAuth successful. UserId: {UserId}, RoleCount: {RoleCount}, CorrelationId: {CorrelationId}", user.Id, roles.Count, correlationId);

            return (response, true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during Google OAuth callback. CorrelationId: {CorrelationId}", correlationId);
            await Task.Delay(Random.Shared.Next(100, 300));
            return (null, false);
        }
    }

    /// <inheritdoc />
    public async Task<(AuthResponseDto? response, bool succeeded)> RefreshTokenAsync(string refreshToken)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Attempting to refresh token. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            var existingToken = await refreshTokenService.GetByRefreshTokenAsync(refreshToken);


            if (existingToken is null || existingToken.ExpiresAt < DateTime.UtcNow)
            {
                logger.LogWarning("Refresh token expired. CorrelationId: {CorrelationId}", correlationId);
                return (null, false);
            }

            await refreshTokenService.RevokeRefreshTokenAsync(existingToken.Id);

            var user = await userManager.FindByIdAsync(existingToken.UserId);

            if (user is null)
            {
                logger.LogWarning("User associated with refresh token not found. CorrelationId: {CorrelationId}", correlationId);
                return (null, false);
            }

            var roles = await userManager.GetRolesAsync(user);

            var (newToken, newRefreshToken) = await GenerateAndSaveTokensAsync(user, roles);

            var response = CreateAuthResponse(newToken, newRefreshToken, user.UserName!, user.Email!, roles);

            logger.LogInformation("Token refreshed successfully for user {UserId}. CorrelationId: {CorrelationId}", user.Id, correlationId);
            return (response, true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during token refresh. CorrelationId: {CorrelationId}", correlationId);
            return (null, false);
        }
    }

    /// <summary>
    /// Generates JWT and refresh tokens for a user and saves the refresh token to the database.
    /// </summary>
    /// <param name="user">The user for whom to generate tokens.</param>
    /// <param name="roles">The roles assigned to the user.</param>
    /// <param name="displayName">Optional display name for the token (used in Google OAuth).</param>
    /// <param name="pictureUrl">Optional picture URL for the token (used in Google OAuth).</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A tuple containing the generated JWT token and refresh token.</returns>
    private async Task<(string token, string refreshToken)> GenerateAndSaveTokensAsync(IdentityUser user, IList<string> roles, string? displayName = null, string?
    pictureUrl = null, CancellationToken cancellationToken = default)
    {
        var token = jwtTokenService.GenerateToken(user, roles, displayName, pictureUrl);
        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var expirySeconds = (int)TimeSpan.FromDays(jwtSettings.RefreshTokenExpiryDays).TotalSeconds;

        await refreshTokenService.SaveRefreshTokenAsync(user.Id, refreshToken, expirySeconds, cancellationToken);

        return (token, refreshToken);
    }

    /// <summary>
    /// Creates an authentication response DTO with the provided tokens and user information.
    /// </summary>
    /// <param name="token">The JWT access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="username">The username to include in the response.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="roles">The user's assigned roles.</param>
    /// <returns>A populated AuthResponseDto instance.</returns>
    private static AuthResponseDto CreateAuthResponse(string token, string refreshToken, string username, string email, IList<string> roles)
    {
        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Username = username,
            Email = email,
            Roles = roles
        };
    }
}