using AutoMapper;
using Backend.DTOs.Orders;
using Backend.Models;
using Backend.Services.Orders.Interfaces;
using Backend.Repository.Interfaces;
using System.Security.Claims;
using System.Diagnostics;

namespace Backend.Services.Orders.Implementations;

/// <summary>
/// Implementation of order service for managing customer orders.
/// 
/// Handles order creation, retrieval, and transformation to DTOs.
/// Ensures service availability validation and accurate pricing calculation.
/// </summary>
/// <param name="orderRepository">The repository for accessing order data.</param>
/// <param name="serviceRepository">The repository for accessing service data.</param>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="logger">The logger instance.</param>
public class OrdersService(IOrderRepository orderRepository, IServiceRepository serviceRepository, IHttpContextAccessor httpContextAccessor, 
IMapper mapper, ILogger<OrdersService> logger) : IOrdersService
{
    /// <inheritdoc />
    public async Task<IList<OrderResponseDto>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Retrieving orders. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            var orders = await orderRepository.GetOrdersAsync(cancellationToken);
            logger.LogInformation("Successfully retrieved {Count} orders. CorrelationId: {CorrelationId}", orders.Count, correlationId);
            return orders;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving orders. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<OrderResponseDto> CreateOrderAsync(OrderDto orderDto, CancellationToken cancellationToken = default)
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Creating new order. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            var serviceIds = orderDto.OrderItems.Select(i => i.ServiceId);

            var servicesList = await serviceRepository.GetByIdsAsync(serviceIds, cancellationToken);

            var services = servicesList.ToDictionary(s => s.Id);

            var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
                if (services.TryGetValue(itemDto.ServiceId, out var service) && service.Available)
                {
                    var orderItem = new OrderItem
                    {
                        ServiceId = service.Id,
                        Quantity = itemDto.Quantity,
                        Price = service.Price, 
                    };

                    newOrder.OrderItems.Add(orderItem);
                    totalAmount += orderItem.Quantity * orderItem.Price;
                }
            }

            newOrder.TotalAmount = totalAmount;

            await orderRepository.AddAsync(newOrder, cancellationToken);

            await orderRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully created order for user {UserId}. CorrelationId: {CorrelationId}", userId, correlationId);

            return mapper.Map<OrderResponseDto>(newOrder);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }
}