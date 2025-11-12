using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations;

public class OrdersService(AppDbContext context) : IOrdersService
{
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
    
    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Service)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
    
    public async Task<OrderResponseDto> CreateOrderAsync(OrderDto orderDto, string userId)
    {
        // Obtener todos los servicios de una vez
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
        
        foreach (var itemDto in orderDto.OrderItems)
        {
            if (services.TryGetValue(itemDto.ServiceId, out var service) &&
                service.Available) // Verificar que el servicio esté disponible
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
        
        // Mapear la entidad Order a OrderResponseDto
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