using Backend.Models;
using Backend.Repository.Interfaces;
using Backend.Services.Auth.Interfaces;
using System.Diagnostics;

namespace Backend.Services.Auth.Implementations;

/// <summary>
/// Service responsible for managing refresh tokens in the database.
/// </summary>
/// <param name="repository">The repository for accessing refresh token data.</param>
/// <param name="logger">The logger instance.</param>
public class RefreshTokenService(IRefreshTokenRepository repository, ILogger<RefreshTokenService> logger, IHttpContextAccessor httpContextAccessor) : IRefreshTokenService
{
    /// <inheritdoc />
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
                var newToken = new UserRefreshToken
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
    public async Task RevokeRefreshTokenAsync(int tokenId, CancellationToken cancellationToken = default)
    {
        var token = await repository.GetByIdAsync(tokenId, cancellationToken);
        
        if (token is not null)
        {
            await repository.DeleteAsync(token, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<UserRefreshToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await repository.GetByTokenAsync(refreshToken, cancellationToken);
    }

}