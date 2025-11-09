using Backend.DTOs;

namespace Backend.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}