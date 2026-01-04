using Backend.Models;

namespace Backend.Repository.Interfaces;

/// <summary>
/// Repository interface for managing refresh tokens.
/// </summary>
public interface IRefreshTokenRepository : IRepository<UserRefreshToken>
{
    /// <summary>
    /// Retrieves a user's refresh token by their user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The user's token if found; otherwise, null.</returns>
    Task<UserRefreshToken?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a refresh token record by the token string.
    /// </summary>
    /// <param name="refreshToken">The refresh token string.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The token record if found; otherwise, null.</returns>
    Task<UserRefreshToken?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
