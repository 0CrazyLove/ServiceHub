using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Implementations;

/// <summary>
/// Implementation of the refresh token repository using Entity Framework Core.
/// </summary>
public class RefreshTokenRepository(AppDbContext context) : Repository<UserRefreshToken>(context), IRefreshTokenRepository
{
    /// <inheritdoc />
    public async Task<UserRefreshToken?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserRefreshToken?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken, cancellationToken);
    }
}
