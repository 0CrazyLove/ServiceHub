using Microsoft.EntityFrameworkCore;

namespace Backend.DTOs;

public class DashboardStatsDto
{
    [Precision(18,2)]
    public decimal TotalSales { get; set; }
    public int ServicetCount { get; set; }
    public int OrderCount { get; set; }
}