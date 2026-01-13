using AutoMapper;
using Backend.Models;
using Backend.DTOs.Services;
using Backend.Services.ServiceManagement.Interfaces;
using Backend.Repository.Interfaces;
using System.Diagnostics;

namespace Backend.Services.ServiceManagement.Implementations;

/// <summary>
/// Implementation of service catalog management.
/// 
/// Handles service queries with filtering and pagination, CRUD operations,
/// and category discovery. Manages JSON serialization for language storage.
/// </summary>
/// <param name="repository">The repository for accessing service data.</param>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
public class ServicesService(IServiceRepository repository, IMapper mapper, ILogger<ServicesService> logger, IHttpContextAccessor httpContextAccessor) : IServiceManagementService
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
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Retrieving services. Category: {Category}, Page: {Page}, PageSize: {PageSize}. CorrelationId: {CorrelationId}", category, page, pageSize, correlationId);

        try
        {
            var (items, totalCount) = await repository.GetServicesAsync(category, page, pageSize, minPrice, maxPrice, cancellationToken);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var serviceDtos = mapper.Map<IList<ServiceResponseDto>>(items);

            logger.LogInformation("Successfully retrieved {Count} services. CorrelationId: {CorrelationId}", items.Count(), correlationId);

            return new PaginatedServicesResponseDto
            {
                Items = serviceDtos,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving services. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific service by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the service to retrieve.</param>
    /// <returns>The service details if found; otherwise, null.</returns>
    public async Task<ServiceResponseDto?> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Retrieving service details. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);

        try
        {
            var service = await repository.GetByIdAsync(id, cancellationToken);
            
            if (service is null)
            {
                logger.LogWarning("Service not found. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);
                return null;
            }

            logger.LogInformation("Successfully retrieved service. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);
            return mapper.Map<ServiceResponseDto>(service);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving service. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);
            throw;
        }
    }

    /// <summary>
    /// Creates a new service in the catalog.
    /// </summary>
    /// <param name="serviceDto">The service details to create.</param>
    /// <returns>The created service details.</returns>
    public async Task<ServiceResponseDto> CreateServiceAsync(ServiceDto serviceDto, CancellationToken cancellationToken = default)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Creating new service. Name: {ServiceName}. CorrelationId: {CorrelationId}", serviceDto.Name, correlationId);

        try
        {
            var newService = mapper.Map<Service>(serviceDto);

            await repository.AddAsync(newService, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully created service. ID: {ServiceId}. CorrelationId: {CorrelationId}", newService.Id, correlationId);

            return mapper.Map<ServiceResponseDto>(newService);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating service. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing service with new details.
    /// </summary>
    /// <param name="id">The ID of the service to update.</param>
    /// <param name="serviceDto">The updated service details.</param>
    /// <returns>The updated service details if found; otherwise, null.</returns>
    public async Task<ServiceResponseDto?> UpdateServiceAsync(int id, ServiceDto serviceDto, CancellationToken cancellationToken = default)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Updating service. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);

        try
        {
            var service = await repository.GetByIdAsync(id, cancellationToken);
            
            if (service is null)
            {
                logger.LogWarning("Service not found for update. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);
                return null;
            }

            mapper.Map(serviceDto, service);

            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully updated service. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);

            return mapper.Map<ServiceResponseDto>(service);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating service. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);
            throw;
        }
    }

    /// <summary>
    /// Deletes a service from the catalog.
    /// </summary>
    /// <param name="id">The ID of the service to delete.</param>
    /// <returns>True if the service was deleted; false if not found.</returns>
    public async Task<bool> DeleteServiceAsync(int id, CancellationToken cancellationToken = default)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Deleting service. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);

        try
        {
            var service = await repository.GetByIdAsync(id, cancellationToken);
            if (service is null)
            {
                logger.LogWarning("Service not found for deletion. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);
                return false;
            }

            await repository.DeleteAsync(service, cancellationToken);

            logger.LogInformation("Successfully deleted service. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting service. ID: {ServiceId}. CorrelationId: {CorrelationId}", id, correlationId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all unique service categories available in the catalog.
    /// </summary>
    /// <returns>A list of unique category names, sorted alphabetically.</returns>
    public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Retrieving service categories. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            var categories = await repository.GetCategoriesAsync(cancellationToken);
            logger.LogInformation("Successfully retrieved {Count} categories. CorrelationId: {CorrelationId}", categories.Count(), correlationId);
            return categories;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving categories. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }
}
