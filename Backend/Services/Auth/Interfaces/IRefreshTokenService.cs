using Backend.Models;

namespace Backend.Services.Auth.Interfaces;

/// <summary>
/// Operations for persisting, retrieving and revoking user refresh tokens.
/// Implementations are responsible for the storage and lifecycle semantics of refresh
/// tokens used to obtain new access tokens without re-authenticating the user.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Creates or updates a refresh token for the specified user.
    /// If a refresh token already exists for the user it will be replaced with the
    /// provided token and its expiration will be updated; otherwise a new record
    /// will be created.
    /// </summary>
    /// <param name="userId">The unique identifier of the user that owns the token.</param>
    /// <param name="refreshToken">The opaque refresh token string to persist.</param>
    /// <param name="expiresIn">
    /// Lifetime of the token in seconds from the current UTC time. Implementations
    /// should convert this relative value to an absolute expiration timestamp and
    /// persist it alongside the token.
    /// </param>
    /// <param name="cancellationToken">Optional token to cancel the asynchronous operation.</param>
    /// <returns>A task that completes once the token has been stored.</returns>1
    Task SaveRefreshTokenAsync(string userId, string refreshToken, int expiresIn, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a previously issued refresh token by its identifier.
    /// </summary>
    /// <param name="tokenId">The database identifier of the refresh token to revoke.</param>
    /// <param name="cancellationToken">Optional token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that completes when the token has been revoked or when the operation
    /// is a no-op (for example, if the token does not exist).
    /// </returns>
    Task RevokeRefreshTokenAsync(int tokenId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a refresh token record by its token string.
    /// </summary>
    /// <param name="refreshToken">The refresh token value to look up.</param>
    /// <param name="cancellationToken">Optional token to cancel the asynchronous operation.</param>
    /// <returns>
    /// The matching <see cref="UserRefreshToken"/> if found; otherwise <c>null</c>.
    /// </returns>
    Task<UserRefreshToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

}