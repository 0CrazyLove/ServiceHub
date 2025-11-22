using Backend.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations;

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
    /// Register a new user account with email and password.
    /// </summary>
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
    /// Authenticate a user with email and password.
    /// Implements timing attack mitigation by adding random delays on failure.
    /// </summary>
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

            logger.LogInformation("Login successful. UserId: {UserId}, Roles: {RoleCount}, CorrelationId: {CorrelationId}",
                user.Id, roles.Count, correlationId);

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
    /// Orchestrates the complete OAuth flow including token exchange, user creation/lookup,
    /// refresh token storage, and JWT generation.
    /// </summary>
    public async Task<(AuthResponseDto? response, bool succeeded)> GoogleCallbackAsync(string authorizationCode)
    {
        try
        {
            // Step 1: Exchange authorization code for Google tokens
            var tokenResponse = await googleAuthService.ExchangeCodeForTokensAsync(authorizationCode);

            if (tokenResponse.IdToken is null)
            {
                logger.LogWarning("Google token response missing ID token");
                return (null, false);
            }

            // Step 2: Validate and decode ID token to get user information
            var userInfo = await googleAuthService.DecodeAndValidateIdTokenAsync(tokenResponse.IdToken);

            // Step 3: Find existing user or create new one
            var (user, succeeded) = await googleAuthService.FindOrCreateGoogleUserAsync(userInfo);

            if (!succeeded || user == null)
            {
                logger.LogError("Failed to find or create Google user for email: {Email}", userInfo.Email);
                return (null, false);
            }

            // Step 4: Store refresh token if provided
            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                await refreshTokenService.SaveRefreshTokenAsync(user.Id, tokenResponse.RefreshToken, tokenResponse.ExpiresIn);
            }

            // Step 5: Update user claims with Google profile information
            await googleAuthService.UpdateGoogleClaimsAsync(user, userInfo);

            // Step 6: Generate JWT and return response
            var roles = await userManager.GetRolesAsync(user);
            var token = jwtTokenService.GenerateToken(user, roles, userInfo.Name, userInfo.Picture);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles
            };

            logger.LogInformation("Google OAuth successful. UserId: {UserId}, Email: {Email}", user.Id, user.Email);
            return (response, true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during Google OAuth callback");
            return (null, false);
        }
    }
}