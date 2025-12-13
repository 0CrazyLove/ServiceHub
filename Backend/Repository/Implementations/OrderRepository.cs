using Backend.Data;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Backend.DTOs.Orders;

namespace Backend.Repository.Implementations;

/// <summary>
/// Implementation of the order repository using Entity Framework Core.
/// </summary>
public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<OrderResponseDto>> GetOrdersWithItemsAsDtoAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Select(o => new OrderResponseDto
        {
            Id = o.Id,
            UserId = o.UserId,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            OrderItems = o.OrderItems.Select(oi => new OrderItemResponseDto
            {
                Id = oi.Id,
                ServiceId = oi.ServiceId,
                ServiceName = oi.Service!.Name,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList()
        }).AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<decimal> GetTotalSalesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.SumAsync(o => o.TotalAmount, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }
}
