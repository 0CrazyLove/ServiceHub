using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Backend.Services.Auth.Interfaces;

namespace Backend.Services.Auth.Implementations;

/// <summary>
/// Service responsible for managing Google refresh tokens in the database.
/// </summary>
public class RefreshTokenService(AppDbContext context, ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    /// <summary>
    /// Store or update Google refresh token for a user.
    /// Creates new record or updates existing one with new token and expiration time.
    /// </summary>
    public async Task SaveRefreshTokenAsync(string userId, string refreshToken, int expiresIn)
    {
        var existingToken = await context.UserGoogleTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        
        if (existingToken != null)
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
            context.UserGoogleTokens.Add(newToken);
            logger.LogDebug("Created new refresh token for user: {UserId}", userId);
        }
        
        await context.SaveChangesAsync();
    }
}