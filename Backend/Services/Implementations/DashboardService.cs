using Backend.Data;
using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations;

public class DashboardService(AppDbContext context) : IDashboardService
{
    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalSales = await context.Orders.SumAsync(o => o.TotalAmount);
        var productCount = await context.Products.CountAsync();
        var orderCount = await context.Orders.CountAsync();

        return new DashboardStatsDto
        {
            TotalSales = totalSales,
            ProductCount = productCount,
            OrderCount = orderCount
        };
    }
}