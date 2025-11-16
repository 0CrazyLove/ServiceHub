using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

/// <summary>
/// Represents a service offering in the ServiceHub marketplace.
/// 
/// Services are the core business entities that vendors provide to customers.
/// Each service includes detailed information about pricing, quality metrics,
/// availability, and delivery capabilities. Services support flexible pricing
/// models (project-based, hourly, monthly, etc.) and are categorized for discovery.
/// </summary>
public class Service
{
    /// <summary>
    /// Gets or sets the unique identifier for the service.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the service name.
    /// Required and limited to 200 characters for database indexing efficiency.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of the service.
    /// Optional field providing comprehensive information about what the service includes.
    /// Limited to 1000 characters.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the base price of the service.
    /// Must be a positive value with up to 2 decimal places for currency representation.
    /// </summary>
    [Precision(18, 2)]
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the pricing model type.
    /// Examples: "project", "hour", "month", "package".
    /// Defaults to "project" if not specified.
    /// </summary>
    [MaxLength(50)]
    public string PriceType { get; set; } = "project";

    /// <summary>
    /// Gets or sets the service category for filtering and discovery.
    /// Examples: "web-development", "graphic-design", "consulting", etc.
    /// Limited to 100 characters.
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the name or identifier of the service provider/vendor.
    /// Required and limited to 200 characters.
    /// </summary>
    [MaxLength(200)]
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the average rating of the service on a 0-5 scale.
    /// Stored with 2 decimal places precision for granular scoring.
    /// </summary>
    [Precision(3, 2)]
    public decimal Rating { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of reviews/ratings received.
    /// Used alongside Rating to calculate weighted averages and display credibility.
    /// </summary>
    public int ReviewCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of successfully completed jobs.
    /// Indicates the vendor's experience and track record.
    /// </summary>
    public int CompletedJobs { get; set; } = 0;

    /// <summary>
    /// Gets or sets the expected delivery time for the service.
    /// Format examples: "1-2 days", "24 hours", "1 week", "2-3 weeks".
    /// Limited to 100 characters.
    /// </summary>
    [MaxLength(100)]
    public string DeliveryTime { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to the service image or thumbnail.
    /// Used in the UI to display service previews.
    /// Limited to 500 characters to accommodate long URLs.
    /// </summary>
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets whether the service provider has been verified.
    /// Indicates platform vetting or certification status.
    /// </summary>
    public bool Verified { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the service is currently available for purchase.
    /// Allows vendors to temporarily disable services without deletion.
    /// </summary>
    public bool Available { get; set; } = true;

    /// <summary>
    /// Gets or sets the supported languages for this service as a JSON array string.
    /// Format: ["Spanish", "English", "French"] serialized as JSON.
    /// Limited to 500 characters.
    /// </summary>
    [MaxLength(500)]
    public string Languages { get; set; } = "[]";

    /// <summary>
    /// Gets or sets the UTC timestamp when the service was created.
    /// Automatically set to current UTC time on creation.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the UTC timestamp when the service was last updated.
    /// Automatically updated on any modification.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}