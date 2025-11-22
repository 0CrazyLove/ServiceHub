using Backend.DTOs.Orders;
using Backend.Models;

namespace Backend.Services.Orders.Interfaces;

/// <summary>
/// Service interface for order management operations.
/// 
/// Defines the contract for creating, retrieving, and managing customer orders.
/// </summary>
public interface IOrdersService
{
    /// <summary>
    /// Retrieve all orders in the system.
    /// 
    /// Note: In production, this should be filtered by user to return only
    /// the authenticated user's orders.
    /// </summary>
    /// <returns>A collection of OrderResponseDto objects.</returns>
    Task<IEnumerable<OrderResponseDto>> GetOrdersAsync();

    /// <summary>
    /// Retrieve a specific order by ID.
    /// </summary>
    /// <param name="id">The order ID to retrieve.</param>
    /// <returns>
    /// The Order entity if found; null otherwise.
    /// Note: Includes related OrderItems and Service information via navigation properties.
    /// </returns>
    Task<Order?> GetOrderByIdAsync(int id);

    /// <summary>
    /// Create a new order for the specified user.
    /// 
    /// Validates that all services are available, creates order items with
    /// current pricing, and calculates the total amount.
    /// </summary>
    /// <param name="orderDto">Order data containing items (services and quantities).</param>
    /// <param name="userId">The user ID creating the order.</param>
    /// <returns>OrderResponseDto representing the created order.</returns>
    Task<OrderResponseDto> CreateOrderAsync(OrderDto orderDto, string userId);
}
