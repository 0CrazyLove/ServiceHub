namespace Backend.Services.Auth.Interfaces;

/// <summary>
/// Service for managing Google refresh tokens.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Save or update a user's Google refresh token.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="refreshToken">Google refresh token</param>
    /// <param name="expiresIn">Token expiration time in seconds</param>
    Task SaveRefreshTokenAsync(string userId, string refreshToken, int expiresIn);
}