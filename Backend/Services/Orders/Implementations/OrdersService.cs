using Backend.DTOs.Orders;
using Backend.Models;
using Backend.Services.Orders.Interfaces;
using Backend.Repository.Interfaces;

namespace Backend.Services.Orders.Implementations;

/// <summary>
/// Implementation of order service for managing customer orders.
/// 
/// Handles order creation, retrieval, and transformation to DTOs.
/// Ensures service availability validation and accurate pricing calculation.
/// </summary>
/// <param name="orderRepository">The repository for accessing order data.</param>
/// <param name="serviceRepository">The repository for accessing service data.</param>
public class OrdersService(IOrderRepository orderRepository, IServiceRepository serviceRepository) : IOrdersService
{
    /// <inheritdoc />
    public async Task<IEnumerable<OrderResponseDto>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.GetOrdersWithItemsAsync(cancellationToken);

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

    /// <inheritdoc />
    public async Task<Order?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await orderRepository.GetOrderByIdWithItemsAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OrderResponseDto> CreateOrderAsync(OrderDto orderDto, string userId, CancellationToken cancellationToken = default)
    {
        // Fetch all services in one query for efficiency
        var serviceIds = orderDto.OrderItems.Select(i => i.ServiceId).ToList();
        var servicesList = await serviceRepository.GetByIdsAsync(serviceIds, cancellationToken);
        var services = servicesList.ToDictionary(s => s.Id);

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
        await orderRepository.AddAsync(newOrder,cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

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