using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;
using Backend.DTOs;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var stats = await dashboardService.GetDashboardStatsAsync();
        return Ok(stats);
    }
}
