namespace Backend.DTOs;

/// <summary>
/// Data Transfer Object for paginated service results.
/// 
/// Wraps a collection of services with pagination metadata.
/// Used when retrieving services with filtering and sorting to provide
/// the client with navigation information for multi-page results.
/// </summary>
public class PaginatedServicesDto
{
    /// <summary>
    /// Gets or sets the collection of services in the current page.
    /// </summary>
    public List<ServiceResponseDto> Items { get; set; } = [];

    /// <summary>
    /// Gets or sets the total number of available pages.
    /// Calculated based on total count and page size.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets the current page number being returned (1-based).
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items matching the query criteria.
    /// </summary>
    public int TotalCount { get; set; }
}
