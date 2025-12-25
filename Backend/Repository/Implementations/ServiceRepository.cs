using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Implementations;

/// <summary>
/// Implementation of the service repository using Entity Framework Core.
/// </summary>
public class ServiceRepository(AppDbContext context) : Repository<Service>(context), IServiceRepository
{
    /// <inheritdoc />
    public async Task<(IEnumerable<Service> Items, int TotalCount)> GetServicesAsync(string? category, int page, int pageSize, decimal? minPrice, decimal? maxPrice, 
    CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();
        
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(s => s.Category == category);
        }
        
        if (minPrice.HasValue)
        {
            query = query.Where(s => s.Price >= minPrice.Value);
        }
        
        if (maxPrice.HasValue)
        {
            query = query.Where(s => s.Price <= maxPrice.Value);
        }
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query.OrderByDescending(s => s.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            
        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(s => !string.IsNullOrEmpty(s.Category)).Select(s => s.Category!).Distinct().OrderBy(c => c).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IList<Service>> GetByIdsAsync(IEnumerable<int> ids,CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(s => ids.Contains(s.Id)).ToListAsync(cancellationToken);
    }
}
