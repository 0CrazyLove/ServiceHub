using Backend.Models;
using Backend.Repository.Interfaces;
using Backend.Services.Auth.Interfaces;
using System.Diagnostics;

namespace Backend.Services.Auth.Implementations;

/// <summary>
/// Service responsible for managing Google refresh tokens in the database.
/// </summary>
/// <param name="repository">The repository for accessing refresh token data.</param>
/// <param name="logger">The logger instance.</param>
public class RefreshTokenService(IRefreshTokenRepository repository, ILogger<RefreshTokenService> logger, IHttpContextAccessor httpContextAccessor) : IRefreshTokenService
{
    /// <summary>
    /// Store or update Google refresh token for a user.
    /// Creates new record or updates existing one with new token and expiration time.
    /// </summary>
    public async Task SaveRefreshTokenAsync(string userId, string refreshToken, int expiresIn, CancellationToken cancellationToken = default)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Saving refresh token for user {UserId}. CorrelationId: {CorrelationId}", userId, correlationId);

        try
        {
            var existingToken = await repository.GetByUserIdAsync(userId, cancellationToken);

            if (existingToken is not null)
            {
                existingToken.RefreshToken = refreshToken;
                existingToken.ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
                logger.LogDebug("Updated existing refresh token for user {UserId}. CorrelationId: {CorrelationId}", userId, correlationId);
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
                logger.LogDebug("Created new refresh token record for user {UserId}. CorrelationId: {CorrelationId}", userId, correlationId);
            }

            await repository.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully saved refresh token for user {UserId}. CorrelationId: {CorrelationId}", userId, correlationId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving refresh token for user {UserId}. CorrelationId: {CorrelationId}", userId, correlationId);
            throw;
        }
    }
}