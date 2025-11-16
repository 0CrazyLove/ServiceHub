namespace Backend.DTOs;

/// <summary>
/// Data Transfer Object for service responses.
/// 
/// Returned when retrieving service details from the API.
/// Contains complete information about a service including metadata
/// like creation date and languages supported. This DTO is used for
/// both single service retrieval and within paginated results.
/// </summary>
public class ServiceResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the service.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of the service.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the price of the service.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the pricing model type (project, hour, month, etc.).
    /// </summary>
    public string PriceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category for filtering and discovery.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the vendor or provider name.
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the average rating on a 0-5 scale.
    /// </summary>
    public decimal Rating { get; set; }

    /// <summary>
    /// Gets or sets the number of reviews received.
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Gets or sets the number of completed projects by this vendor.
    /// </summary>
    public int CompletedJobs { get; set; }

    /// <summary>
    /// Gets or sets the expected delivery timeframe.
    /// </summary>
    public string DeliveryTime { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to the service image or thumbnail.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets whether the provider is verified.
    /// </summary>
    public bool Verified { get; set; }

    /// <summary>
    /// Gets or sets whether the service is currently available.
    /// </summary>
    public bool Available { get; set; }

    /// <summary>
    /// Gets or sets the list of supported languages.
    /// </summary>
    public List<string> Languages { get; set; } = [];

    /// <summary>
    /// Gets or sets the UTC timestamp when the service was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the service was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}