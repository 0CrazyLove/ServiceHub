using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;
using Backend.DTOs;

namespace Backend.Controllers;

/// <summary>
/// API controller for dashboard and administrative statistics.
/// 
/// Provides aggregated metrics and KPIs for platform administrators.
/// All endpoints require AdminPolicy authorization.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminPolicy")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    /// <summary>
    /// Retrieve aggregated dashboard statistics (admin only).
    /// 
    /// Returns key performance indicators including:
    /// - Total sales across all orders
    /// - Total number of services in the catalog
    /// - Total number of orders placed
    /// 
    /// Requires AdminPolicy authorization.
    /// </summary>
    /// <returns>DashboardStatsDto containing platform statistics.</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var stats = await dashboardService.GetDashboardStatsAsync();
        return Ok(stats);
    }
}
