using Backend.Models;
using Backend.DTOs.Services;
using System.Text.Json;
using Backend.Services.Business.Interfaces;
using Backend.Repository.Interfaces;

namespace Backend.Services.Business.Implemetations;

/// <summary>
/// Implementation of service catalog management.
/// 
/// Handles service queries with filtering and pagination, CRUD operations,
/// and category discovery. Manages JSON serialization for language storage.
/// </summary>
/// <param name="repository">The repository for accessing service data.</param>
public class ServicesService(IServiceRepository repository) : IServicesService
{
    /// <summary>
    /// Retrieves a paginated list of services with optional filtering by category and price.
    /// </summary>
    /// <param name="category">The category to filter by (optional).</param>
    /// <param name="page">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="minPrice">The minimum price filter (optional).</param>
    /// <param name="maxPrice">The maximum price filter (optional).</param>
    /// <returns>A DTO containing the list of services and pagination metadata.</returns>
    public async Task<PaginatedServicesDto> GetServices(string? category, int page, int pageSize,decimal? minPrice,decimal? maxPrice)
    {
        var (items, totalCount) = await repository.GetServicesAsync(category, page, pageSize, minPrice, maxPrice);
        
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var serviceDtos = items.Select(MapToResponseDto).ToList();
        
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
    /// Retrieves a specific service by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the service to retrieve.</param>
    /// <returns>The service details if found; otherwise, null.</returns>
    public async Task<ServiceResponseDto?> GetServiceById(int id)
    {
        var service = await repository.GetByIdAsync(id);
        return service == null ? null : MapToResponseDto(service);
    }

    /// <summary>
    /// Creates a new service in the catalog.
    /// </summary>
    /// <param name="serviceDto">The service details to create.</param>
    /// <returns>The created service details.</returns>
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
        
        await repository.AddAsync(newService);
        await repository.SaveChangesAsync();
        
        return MapToResponseDto(newService);
    }

    /// <summary>
    /// Updates an existing service with new details.
    /// </summary>
    /// <param name="id">The ID of the service to update.</param>
    /// <param name="serviceDto">The updated service details.</param>
    /// <returns>The updated service details if found; otherwise, null.</returns>
    public async Task<ServiceResponseDto?> UpdateService(int id, ServiceDto serviceDto)
    {
        var service = await repository.GetByIdAsync(id);
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
        
        await repository.SaveChangesAsync();
        
        return MapToResponseDto(service);
    }

    /// <summary>
    /// Deletes a service from the catalog.
    /// </summary>
    /// <param name="id">The ID of the service to delete.</param>
    /// <returns>True if the service was deleted; false if not found.</returns>
    public async Task<bool> DeleteService(int id)
    {
        var service = await repository.GetByIdAsync(id);
        if (service == null) return false;
        
        repository.Remove(service);
        await repository.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Retrieves all unique service categories available in the catalog.
    /// </summary>
    /// <returns>A list of unique category names, sorted alphabetically.</returns>
    public async Task<IEnumerable<string>> GetCategories()
    {
        return await repository.GetCategoriesAsync();
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
