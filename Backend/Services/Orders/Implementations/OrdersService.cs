using Backend.Data;
using Backend.DTOs.Orders;
using Backend.Models;
using Backend.Services.Orders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Orders.Implementations;

/// <summary>
/// Implementation of order service for managing customer orders.
/// 
/// Handles order creation, retrieval, and transformation to DTOs.
/// Ensures service availability validation and accurate pricing calculation.
/// </summary>
public class OrdersService(AppDbContext context) : IOrdersService
{
    /// <summary>
    /// Retrieve all orders with related items and services.
    /// 
    /// Transforms Order entities to OrderResponseDto for API responses.
    /// </summary>
    public async Task<IEnumerable<OrderResponseDto>> GetOrdersAsync()
    {
        var orders = await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Service)
            .ToListAsync();
            
        return orders.Select(o => new OrderResponseDto
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            OrderItems = o.OrderItems.Select(oi => new OrderItemResponseDto
            {
                Id = oi.Id,
                ServiceId = oi.ServiceId,
                ServiceName = oi.Service?.Name,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList()
        }).ToList();
    }

    /// <summary>
    /// Retrieve a specific order with all related items and services.
    /// </summary>
    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Service)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <summary>
    /// Create a new order for the specified user.
    /// 
    /// Process:
    /// 1. Fetch all requested services in a single query
    /// 2. Validate that services exist and are available
    /// 3. Create order items with current pricing
    /// 4. Calculate total amount
    /// 5. Persist to database
    /// </summary>
    public async Task<OrderResponseDto> CreateOrderAsync(OrderDto orderDto, string userId)
    {
        // Fetch all services in one query for efficiency
        var serviceIds = orderDto.OrderItems.Select(i => i.ServiceId).ToList();
        var services = await context.Services
            .Where(s => serviceIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id);
        
        var newOrder = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            OrderItems = []
        };
        
        decimal totalAmount = 0;
        
        // Add order items, validating service availability
        foreach (var itemDto in orderDto.OrderItems)
        {
            if (services.TryGetValue(itemDto.ServiceId, out var service) &&
                service.Available)
            {
                var orderItem = new OrderItem
                {
                    ServiceId = service.Id,
                    Quantity = itemDto.Quantity,
                    Price = service.Price
                };
                
                newOrder.OrderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.Price;
            }
        }
        
        newOrder.TotalAmount = totalAmount;
        context.Orders.Add(newOrder);
        await context.SaveChangesAsync();
        
        // Transform to DTO for response
        var response = new OrderResponseDto
        {
            Id = newOrder.Id,
            OrderDate = newOrder.OrderDate,
            TotalAmount = newOrder.TotalAmount,
            OrderItems = [..newOrder.OrderItems.Select(oi => new OrderItemResponseDto
            {
                Id = oi.Id,
                ServiceId = oi.ServiceId,
                ServiceName = services[oi.ServiceId].Name,
                Quantity = oi.Quantity,
                Price = oi.Price
            })]
        };
        
        return response;
    }
}