using Backend.DTOs.Dashboard;
using Backend.Services.Dashboard.Interfaces;
using Backend.Repository.Interfaces;

namespace Backend.Services.Dashboard.Implementations;

/// <summary>
/// Implementation of dashboard service for retrieving platform statistics.
/// 
/// Aggregates data from orders and services to provide KPIs for administrators.
/// </summary>
/// <param name="orderRepository">The repository for accessing order data.</param>
/// <param name="serviceRepository">The repository for accessing service data.</param>
public class DashboardService(IOrderRepository orderRepository, IServiceRepository serviceRepository) : IDashboardService
{
    /// <summary>
    /// Retrieves aggregated dashboard statistics including sales and counts.
    /// </summary>
    /// <returns>A DTO containing the calculated platform statistics.</returns>
    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken)
    {
        var totalSales = await orderRepository.GetTotalSalesAsync(cancellationToken);
        var serviceCount = await serviceRepository.GetCountAsync(cancellationToken);
        var orderCount = await orderRepository.GetCountAsync(cancellationToken);

        return new DashboardStatsDto
        {
            TotalSales = totalSales,
            ServicetCount = serviceCount,
            OrderCount = orderCount
        };
    }
}