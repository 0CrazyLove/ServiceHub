using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Services;

/// <summary>
/// Data Transfer Object for service creation and update requests.
/// 
/// Contains all the information needed to create or modify a service offering.
/// This DTO is used as input for both POST (create) and PUT (update) operations.
/// Includes comprehensive validation attributes to ensure data integrity.
/// </summary>
public class ServiceDto
{
    /// <summary>
    /// Gets or sets the name of the service.
    /// Required and validated at API boundary.
    /// </summary>
    [Required(ErrorMessage = "Service name is required")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of what the service provides.
    /// Optional field allowing up to 1000 characters.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the base price for the service.
    /// Must be greater than zero with currency precision.
    /// </summary>
    [Required]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than zero")]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the pricing model type.
    /// Examples: "project", "hour", "month", "package"
    /// Defaults to "project".
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string PriceType { get; set; } = "project";

    /// <summary>
    /// Gets or sets the service category for filtering and discovery.
    /// Examples: "web-development", "graphic-design", "consulting"
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the vendor or provider name.
    /// Required and limited to 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Provider name is required")]
    [MaxLength(200)]
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the average rating on a 0-5 scale.
    /// Validates that the rating is within acceptable bounds.
    /// </summary>
    [Range(0, 5)]
    [Precision(3, 2)]
    public decimal Rating { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of reviews received.
    /// Validates non-negative values.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int ReviewCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of successfully completed projects or jobs.
    /// Indicates vendor experience and reliability.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int CompletedJobs { get; set; } = 0;

    /// <summary>
    /// Gets or sets the expected delivery timeframe.
    /// Format examples: "1-2 days", "24 hours", "1 week"
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DeliveryTime { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to the service's image or thumbnail.
    /// Used for UI display purposes.
    /// </summary>
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets whether the service provider has been verified by the platform.
    /// </summary>
    public bool Verified { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the service is currently available for purchase.
    /// </summary>
    public bool Available { get; set; } = true;

    /// <summary>
    /// Gets or sets the list of languages supported for this service.
    /// Examples: ["Spanish", "English", "French"]
    /// Will be serialized to JSON format in the database.
    /// </summary>
    public List<string> Languages { get; set; } = new();
}
