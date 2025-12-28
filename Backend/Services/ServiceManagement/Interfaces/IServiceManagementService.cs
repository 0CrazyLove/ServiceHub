using Backend.DTOs.Services;

namespace Backend.Services.ServiceManagement.Interfaces;

/// <summary>
/// Service interface for service catalog management operations.
/// 
/// Defines the contract for querying, creating, updating, and deleting services
/// in the ServiceHub marketplace.
/// </summary>
public interface IServicesService
{
    /// <summary>
    /// Retrieve a paginated list of services with optional filtering.
    /// 
    /// Supports filtering by category and price range. Results are sorted by
    /// creation date (newest first).
    /// </summary>
    /// <param name="category">Optional category filter (exact match).</param>
    /// <param name="page">Page number for pagination (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="minPrice">Optional minimum price filter (inclusive).</param>
    /// <param name="maxPrice">Optional maximum price filter (inclusive).</param>
    /// <returns>PaginatedServicesDto with services and pagination metadata.</returns>
    Task<PaginatedServicesResponseDto> GetServicesAsync(string? category, int page, int pageSize, decimal? minPrice, decimal? maxPrice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve detailed information about a specific service.
    /// </summary>
    /// <param name="id">The service ID to retrieve.</param>
    /// <returns>ServiceResponseDto if found; null otherwise.</returns>
    Task<ServiceResponseDto?> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new service.
    /// </summary>
    /// <param name="serviceDto">The service details to create.</param>
    /// <returns>ServiceResponseDto representing the created service with generated ID.</returns>
    Task<ServiceResponseDto> CreateServiceAsync(ServiceDto serviceDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing service.
    /// </summary>
    /// <param name="id">The ID of the service to update.</param>
    /// <param name="serviceDto">The updated service details.</param>
    /// <returns>
    /// ServiceResponseDto with updated data if found; null if service does not exist.
    /// </returns>
    Task<ServiceResponseDto?> UpdateServiceAsync(int id, ServiceDto serviceDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a service.
    /// </summary>
    /// <param name="id">The ID of the service to delete.</param>
    /// <returns>
    /// True if deletion was successful; false if service does not exist.
    /// </returns>
    Task<bool> DeleteServiceAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve all available service categories.
    /// 
    /// Returns a sorted list of unique category names currently in use
    /// by at least one service.
    /// </summary>
    /// <returns>An enumerable collection of category names.</returns>
    Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
