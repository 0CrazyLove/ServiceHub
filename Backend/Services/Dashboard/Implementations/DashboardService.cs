using Backend.DTOs.Dashboard;
using Backend.Services.Dashboard.Interfaces;
using Backend.Repository.Interfaces;

using System.Diagnostics;

namespace Backend.Services.Dashboard.Implementations;

/// <summary>
/// Implementation of dashboard service for retrieving platform statistics.
/// 
/// Aggregates data from orders and services to provide KPIs for administrators.
/// </summary>
/// <param name="orderRepository">The repository for accessing order data.</param>
/// <param name="serviceRepository">The repository for accessing service data.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
public class DashboardService(IOrderRepository orderRepository, IServiceRepository serviceRepository, ILogger<DashboardService> logger, IHttpContextAccessor httpContextAccessor) : IDashboardService
{
    /// <summary>
    /// Retrieves aggregated dashboard statistics including sales and counts.
    /// </summary>
    /// <returns>A DTO containing the calculated platform statistics.</returns>
    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Retrieving dashboard statistics. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            var totalSales = await orderRepository.GetTotalSalesAsync(cancellationToken);
            var serviceCount = await serviceRepository.GetCountAsync(cancellationToken);
            var orderCount = await orderRepository.GetCountAsync(cancellationToken);

            logger.LogInformation("Dashboard statistics retrieved successfully. CorrelationId: {CorrelationId}", correlationId);

            return new DashboardStatsDto
            {
                TotalSales = totalSales,
                ServicetCount = serviceCount,
                OrderCount = orderCount
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve dashboard statistics. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }
}