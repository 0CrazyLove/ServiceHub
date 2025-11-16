using Backend.Data;
using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations;

/// <summary>
/// Implementation of dashboard service for retrieving platform statistics.
/// 
/// Aggregates data from orders and services to provide KPIs for administrators.
/// </summary>
public class DashboardService(AppDbContext context) : IDashboardService
{
    /// <summary>
    /// Retrieve aggregated dashboard statistics.
    /// 
    /// Calculates:
    /// - Total sales: sum of all order amounts
    /// - Service count: total number of services in catalog
    /// - Order count: total number of orders placed
    /// </summary>
    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalSales = await context.Orders.SumAsync(o => o.TotalAmount);
        var serviceCount = await context.Services.CountAsync();
        var orderCount = await context.Orders.CountAsync();

        return new DashboardStatsDto
        {
            TotalSales = totalSales,
            ServicetCount = serviceCount,
            OrderCount = orderCount
        };
    }
}