using Backend.Models;
using Backend.Repository.Interfaces;
using Backend.Services.Auth.Interfaces;

namespace Backend.Services.Auth.Implementations;

/// <summary>
/// Service responsible for managing Google refresh tokens in the database.
/// </summary>
/// <param name="repository">The repository for accessing refresh token data.</param>
/// <param name="logger">The logger instance.</param>
public class RefreshTokenService(IRefreshTokenRepository repository, ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    /// <summary>
    /// Store or update Google refresh token for a user.
    /// Creates new record or updates existing one with new token and expiration time.
    /// </summary>
    public async Task SaveRefreshTokenAsync(string userId, string refreshToken, int expiresIn, CancellationToken cancellationToken = default)
    {
        var existingToken = await repository.GetByUserIdAsync(userId, cancellationToken);

        if (existingToken is not null)
        {
            existingToken.RefreshToken = refreshToken;
            existingToken.ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
            logger.LogDebug("Updated refresh token for user: {UserId}", userId);
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
            await repository.AddAsync(newToken);
            logger.LogDebug("Created new refresh token for user: {UserId}", userId);
        }

        await repository.SaveChangesAsync(cancellationToken);
    }
}