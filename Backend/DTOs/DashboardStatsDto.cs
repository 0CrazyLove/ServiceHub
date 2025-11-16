using Microsoft.EntityFrameworkCore;

namespace Backend.DTOs;

/// <summary>
/// Data Transfer Object for dashboard statistics.
/// 
/// Provides administrative overview metrics including total sales,
/// service count, and order count. Returned by the dashboard endpoint
/// to display key performance indicators in the admin interface.
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// Gets or sets the total sales amount across all orders.
    /// Calculated as the sum of all order total amounts.
    /// </summary>
    [Precision(18, 2)]
    public decimal TotalSales { get; set; }

    /// <summary>
    /// Gets or sets the total number of services in the marketplace.
    /// Indicates the size of the service catalog.
    /// Note: This property name has a typo (ServicetCount) but is kept for backward compatibility.
    /// </summary>
    public int ServicetCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of orders placed.
    /// Indicates platform activity and transaction volume.
    /// </summary>
    public int OrderCount { get; set; }
}