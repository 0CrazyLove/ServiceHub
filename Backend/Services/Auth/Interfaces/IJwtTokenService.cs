using Microsoft.AspNetCore.Identity;

namespace Backend.Services.Auth.Interfaces;
/// <summary>
/// Service for generating and managing JWT tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate a JWT token for a user with their roles and optional profile information.
    /// </summary>
    /// <param name="user">The identity user</param>
    /// <param name="roles">User roles</param>
    /// <param name="displayName">Optional display name (for Google OAuth)</param>
    /// <param name="picture">Optional profile picture URL (for Google OAuth)</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(IdentityUser user, IList<string> roles, string? displayName = null, string? picture = null);

    /// <summary>
    /// Generates a secure random refresh token.
    /// </summary>
    /// <returns>A base64 encoded random string.</returns>
    string GenerateRefreshToken();
}