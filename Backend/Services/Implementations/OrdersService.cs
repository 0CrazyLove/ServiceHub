using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations;

public class OrdersService(AppDbContext context) : IOrdersService
{
    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {

        return await context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> CreateOrderAsync(OrderDto orderDto)
    {

        // Obtener todos los productos de una vez
        var productIds = orderDto.OrderItems.Select(i => i.ProductId).ToList();
        var products = await context.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

        var newOrder = new Order
        {
            UserId = orderDto.UserId,
            OrderDate = DateTime.UtcNow,
            OrderItems = []
        };

        decimal totalAmount = 0;

        foreach (var itemDto in orderDto.OrderItems)
        {
            if (products.TryGetValue(itemDto.ProductId, out var product) && product.Stock >= itemDto.Quantity)
            {
                product.Stock -= itemDto.Quantity;
                
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    Price = product.Price
                };
                
                newOrder.OrderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.Price;
            }
        }

        newOrder.TotalAmount = totalAmount;
        context.Orders.Add(newOrder);
        await context.SaveChangesAsync();

        return newOrder;
    }
}