using Backend.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
namespace Backend.Services.Auth.Interfaces;

/// <summary>
/// Service interface for authentication and authorization operations.
/// 
/// Defines the contract for user registration, login, Google OAuth authentication,
/// and token management functionality.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register a new user account with email and password.
    /// 
    /// Creates a new user with the provided credentials and assigns the Customer role.
    /// </summary>
    /// <param name="model">Registration data (username, email, password).</param>
    /// <returns>
    /// A tuple containing:
    /// - AuthResponseDto with JWT token if registration succeeds (null if fails)
    /// - Boolean indicating success or failure
    /// </returns>
    Task<(AuthResponseDto? response, bool succeeded)> RegisterUserAsync(RegisterDto model);

    /// <summary>
    /// Authenticate a user with email and password credentials.
    /// 
    /// Validates the provided credentials against stored user records.
    /// On success, generates and returns a JWT token for subsequent authenticated requests.
    /// </summary>
    /// <param name="model">Login credentials (email, password).</param>
    /// <returns>
    /// A tuple containing:
    /// - AuthResponseDto with JWT token if authentication succeeds (null if fails)
    /// - Boolean indicating success or failure
    /// </returns>
    Task<(AuthResponseDto? response, bool succeeded)> LoginUserAsync(LoginDto model);

    /// <summary>
    /// Handle Google OAuth callback after user authorization.
    /// 
    /// Exchanges an authorization code for tokens, retrieves user information,
    /// creates or updates the user account, and returns an application JWT token.
    /// </summary>
    /// <param name="code">The authorization code from Google OAuth.</param>
    /// <returns>
    /// A tuple containing:
    /// - AuthResponseDto with JWT token if authentication succeeds (null if fails)
    /// - Boolean indicating success or failure
    /// </returns>
    Task<(AuthResponseDto? response, bool succeeded)> GoogleCallbackAsync(string code, CancellationToken cancellationToken);
}