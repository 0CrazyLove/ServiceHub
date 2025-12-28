using AutoMapper;
using Backend.Models;
using Backend.DTOs.Services;
using Backend.Services.ServiceManagement.Interfaces;
using Backend.Repository.Interfaces;

namespace Backend.Services.ServiceManagement.Implementations;

/// <summary>
/// Implementation of service catalog management.
/// 
/// Handles service queries with filtering and pagination, CRUD operations,
/// and category discovery. Manages JSON serialization for language storage.
/// </summary>
/// <param name="repository">The repository for accessing service data.</param>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
public class ServicesService(IServiceRepository repository, IMapper mapper) : IServicesService
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
    public async Task<PaginatedServicesResponseDto> GetServicesAsync(string? category, int page, int pageSize, decimal? minPrice, decimal? maxPrice,
     CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await repository.GetServicesAsync(category, page, pageSize, minPrice, maxPrice, cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var serviceDtos = mapper.Map<IList<ServiceResponseDto>>(items);

        return new PaginatedServicesResponseDto
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
    public async Task<ServiceResponseDto?> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var service = await repository.GetByIdAsync(id, cancellationToken);
        return service is null ? null : mapper.Map<ServiceResponseDto>(service);
    }

    /// <summary>
    /// Creates a new service in the catalog.
    /// </summary>
    /// <param name="serviceDto">The service details to create.</param>
    /// <returns>The created service details.</returns>
    public async Task<ServiceResponseDto> CreateServiceAsync(ServiceDto serviceDto, CancellationToken cancellationToken = default)
    {
        var newService = mapper.Map<Service>(serviceDto);

        await repository.AddAsync(newService, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<ServiceResponseDto>(newService);
    }

    /// <summary>
    /// Updates an existing service with new details.
    /// </summary>
    /// <param name="id">The ID of the service to update.</param>
    /// <param name="serviceDto">The updated service details.</param>
    /// <returns>The updated service details if found; otherwise, null.</returns>
    public async Task<ServiceResponseDto?> UpdateServiceAsync(int id, ServiceDto serviceDto, CancellationToken cancellationToken = default)
    {
        var service = await repository.GetByIdAsync(id, cancellationToken);
        
        if (service is null) return null;

        mapper.Map<ServiceResponseDto>(service);

        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<ServiceResponseDto>(service);
    }

    /// <summary>
    /// Deletes a service from the catalog.
    /// </summary>
    /// <param name="id">The ID of the service to delete.</param>
    /// <returns>True if the service was deleted; false if not found.</returns>
    public async Task<bool> DeleteServiceAsync(int id, CancellationToken cancellationToken = default)
    {
        var service = await repository.GetByIdAsync(id, cancellationToken);
        if (service is null) return false;

        repository.Remove(service);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Retrieves all unique service categories available in the catalog.
    /// </summary>
    /// <returns>A list of unique category names, sorted alphabetically.</returns>
    public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetCategoriesAsync(cancellationToken);
    }
}
