using Backend.Data;
using Backend.Models;
using Backend.DTOs.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Backend.Services.Business.Interfaces;

namespace Backend.Services.Business.Implemetations;

/// <summary>
/// Implementation of service catalog management.
/// 
/// Handles service queries with filtering and pagination, CRUD operations,
/// and category discovery. Manages JSON serialization for language storage.
/// </summary>
public class ServicesService(AppDbContext context) : IServicesService
{
    /// <summary>
    /// Retrieve paginated services with optional filtering.
    /// 
    /// Applies filters in order: category, price range.
    /// Results sorted by creation date (newest first).
    /// </summary>
    public async Task<PaginatedServicesDto> GetServices(
        string? category, 
        int page, 
        int pageSize,
        decimal? minPrice,
        decimal? maxPrice)
    {
        var query = context.Services.AsQueryable();
        
        // Apply category filter if specified
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(s => s.Category == category);
        }
        
        // Apply minimum price filter if specified
        if (minPrice.HasValue)
        {
            query = query.Where(s => s.Price >= minPrice.Value);
        }
        
        // Apply maximum price filter if specified
        if (maxPrice.HasValue)
        {
            query = query.Where(s => s.Price <= maxPrice.Value);
        }
        
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        // Fetch the requested page, sorted by creation date descending
        var services = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var serviceDtos = services.Select(MapToResponseDto).ToList();
        
        return new PaginatedServicesDto
        {
            Items = serviceDtos,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Retrieve a specific service by ID.
    /// </summary>
    public async Task<ServiceResponseDto?> GetServiceById(int id)
    {
        var service = await context.Services.FindAsync(id);
        return service == null ? null : MapToResponseDto(service);
    }

    /// <summary>
    /// Create a new service.
    /// 
    /// Serializes the language list to JSON format for storage.
    /// Sets creation and update timestamps to current UTC time.
    /// </summary>
    public async Task<ServiceResponseDto> CreateService(ServiceDto serviceDto)
    {
        var newService = new Service
        {
            Name = serviceDto.Name,
            Description = serviceDto.Description,
            Price = serviceDto.Price,
            PriceType = serviceDto.PriceType,
            Category = serviceDto.Category,
            Provider = serviceDto.Provider,
            Rating = serviceDto.Rating,
            ReviewCount = serviceDto.ReviewCount,
            CompletedJobs = serviceDto.CompletedJobs,
            DeliveryTime = serviceDto.DeliveryTime,
            ImageUrl = serviceDto.ImageUrl,
            Verified = serviceDto.Verified,
            Available = serviceDto.Available,
            Languages = JsonSerializer.Serialize(serviceDto.Languages),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        context.Services.Add(newService);
        await context.SaveChangesAsync();
        
        return MapToResponseDto(newService);
    }

    /// <summary>
    /// Update an existing service.
    /// 
    /// Updates all properties and sets UpdatedAt to current UTC time.
    /// Returns null if service not found.
    /// </summary>
    public async Task<ServiceResponseDto?> UpdateService(int id, ServiceDto serviceDto)
    {
        var service = await context.Services.FindAsync(id);
        if (service == null) return null;
        
        service.Name = serviceDto.Name;
        service.Description = serviceDto.Description;
        service.Price = serviceDto.Price;
        service.PriceType = serviceDto.PriceType;
        service.Category = serviceDto.Category;
        service.Provider = serviceDto.Provider;
        service.Rating = serviceDto.Rating;
        service.ReviewCount = serviceDto.ReviewCount;
        service.CompletedJobs = serviceDto.CompletedJobs;
        service.DeliveryTime = serviceDto.DeliveryTime;
        service.ImageUrl = serviceDto.ImageUrl;
        service.Verified = serviceDto.Verified;
        service.Available = serviceDto.Available;
        service.Languages = JsonSerializer.Serialize(serviceDto.Languages);
        service.UpdatedAt = DateTime.UtcNow;
        
        await context.SaveChangesAsync();
        
        return MapToResponseDto(service);
    }

    /// <summary>
    /// Delete a service.
    /// 
    /// Permanently removes the service from the catalog.
    /// Returns false if service not found.
    /// </summary>
    public async Task<bool> DeleteService(int id)
    {
        var service = await context.Services.FindAsync(id);
        if (service == null) return false;
        
        context.Services.Remove(service);
        await context.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Retrieve all available service categories.
    /// 
    /// Returns distinct, non-null categories sorted alphabetically.
    /// </summary>
    public async Task<IEnumerable<string>> GetCategories()
    {
        return await context.Services
            .Where(s => !string.IsNullOrEmpty(s.Category))
            .Select(s => s.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    /// <summary>
    /// Map Service entity to ServiceResponseDto.
    /// 
    /// Deserializes the JSON language string and handles any errors gracefully
    /// by returning an empty list on deserialization failure.
    /// </summary>
    private static ServiceResponseDto MapToResponseDto(Service service)
    {
        List<string> languages;
        try
        {
            languages = JsonSerializer.Deserialize<List<string>>(service.Languages) ?? new List<string>();
        }
        catch
        {
            // Handle JSON deserialization errors gracefully
            languages = new List<string>();
        }
        
        return new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price,
            PriceType = service.PriceType,
            Category = service.Category,
            Provider = service.Provider,
            Rating = service.Rating,
            ReviewCount = service.ReviewCount,
            CompletedJobs = service.CompletedJobs,
            DeliveryTime = service.DeliveryTime,
            ImageUrl = service.ImageUrl,
            Verified = service.Verified,
            Available = service.Available,
            Languages = languages,
            CreatedAt = service.CreatedAt,
            UpdatedAt = service.UpdatedAt
        };
    }
}
