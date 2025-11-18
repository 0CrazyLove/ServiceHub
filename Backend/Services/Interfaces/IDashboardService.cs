using Backend.DTOs.Dashboard;

namespace Backend.Services.Interfaces;

/// <summary>
/// Service interface for dashboard and administrative statistics.
/// 
/// Provides aggregated metrics and key performance indicators for platform admins.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Retrieve aggregated dashboard statistics.
    /// 
    /// Calculates and returns platform-wide metrics including total sales,
    /// service count, and order count.
    /// </summary>
    /// <returns>DashboardStatsDto containing aggregated platform metrics.</returns>
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}