using Backend.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Backend.Services.Auth.Interfaces;

namespace Backend.Services.Auth.Implementations;

/// <summary>
/// Orchestrator service for authentication operations.
/// Coordinates between specialized services to handle user registration, login, and Google OAuth.
/// </summary>
public class AuthService(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IJwtTokenService jwtTokenService,
    IGoogleAuthService googleAuthService,
    IRefreshTokenService refreshTokenService,
    ILogger<AuthService> logger) : IAuthService
{
    /// <summary>
    /// Registers a new user account with the provided email and password.
    /// </summary>
    /// <param name="model">The registration details including username, email, and password.</param>
    /// <returns>A tuple containing the auth response, success status, and any errors.</returns>
    public async Task<(AuthResponseDto? response, bool succeeded, IEnumerable<IdentityError>? Errors)> RegisterUserAsync(RegisterDto model)
    {
        var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

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
                logger.LogWarning("Failed to create user {UserName}. Errors: {ErrorCodes}, CorrelationId: {CorrelationId}",
                    model.UserName, errorCodes, correlationId);
                return (null, false, result.Errors);
            }

            await userManager.AddToRoleAsync(user, "Customer");
            var roles = await userManager.GetRolesAsync(user);
            var token = jwtTokenService.GenerateToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles
            };

            logger.LogInformation("Registration successful. UserId: {UserId}, Roles: {RoleCount}, CorrelationId: {CorrelationId}",
                user.Id, roles.Count, correlationId);

            return (response, true, null);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Critical error during registration: {UserName}, CorrelationId: {CorrelationId}",
                model.UserName, correlationId);

            var errors = new List<IdentityError>
            {
                new()
                {
                    Code = "RegistrationError",
                    Description = "An error occurred during registration. Please try again."
                }
            };

            return (null, false, errors);
        }
    }

    /// <summary>
    /// Authenticates a user with email and password credentials.
    /// </summary>
    /// <param name="model">The login credentials (email and password).</param>
    /// <returns>A tuple containing the auth response and success status.</returns>
    public async Task<(AuthResponseDto? response, bool succeeded)> LoginUserAsync(LoginDto model)
    {
        var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

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
                logger.LogWarning("Login failed. Reason: {Reason}, UserId: {UserId}, CorrelationId: {CorrelationId}",
                    reason, user.Id, correlationId);
                await Task.Delay(Random.Shared.Next(100, 300));
                return (null, false);
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtTokenService.GenerateToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles
            };

            logger.LogInformation("Login successful for user {UserId} with {RoleCount} role(s). CorrelationId: {CorrelationId}", user.Id, roles.Count, correlationId);

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
    /// Handles the Google OAuth callback process.
    /// </summary>
    /// <param name="authorizationCode">The authorization code received from Google.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A tuple containing the auth response and success status.</returns>
    public async Task<(AuthResponseDto? response, bool succeeded)> GoogleCallbackAsync(string authorizationCode, CancellationToken cancellationToken)
    {
        var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

        logger.LogInformation("Starting Google OAuth callback. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            var tokenResponse = await googleAuthService.ExchangeCodeForTokensAsync(authorizationCode , cancellationToken);

            if (string.IsNullOrEmpty(tokenResponse.IdToken))
            {
                logger.LogWarning("Google token response missing ID token. CorrelationId: {CorrelationId}", correlationId);
                return (null, false);
            }
            var userInfo = await googleAuthService.DecodeAndValidateIdTokenAsync(tokenResponse.IdToken, cancellationToken);

            var user = await googleAuthService.FindOrCreateGoogleUserAsync(userInfo);

            if (user is null)
            {
                logger.LogError("Failed to find or create Google user for email: {Email}. CorrelationId: {CorrelationId}", userInfo.Email, correlationId);
                return (null, false);
            }

            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                await refreshTokenService.SaveRefreshTokenAsync(user.Id, tokenResponse.RefreshToken, tokenResponse.ExpiresIn , cancellationToken);
                logger.LogDebug("Refresh token saved for user {UserId}. CorrelationId: {CorrelationId}", user.Id, correlationId);
            }

            await googleAuthService.UpdateGoogleClaimsAsync(user, userInfo);

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtTokenService.GenerateToken(user, roles, userInfo.Name, userInfo.Picture);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles
            };

            logger.LogInformation("Google OAuth successful. UserId: {UserId}, RoleCount: {RoleCount}, CorrelationId: {CorrelationId}", user.Id, roles.Count, correlationId);

            return (response, true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during Google OAuth callback. CorrelationId: {CorrelationId}", correlationId);
            return (null, false);
        }
    }
}