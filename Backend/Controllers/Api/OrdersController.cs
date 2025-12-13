using Microsoft.AspNetCore.Authorization;
using Backend.Models;
using Backend.DTOs.Orders;
using Microsoft.AspNetCore.Mvc;
using Backend.Services.Orders.Interfaces;

namespace Backend.Controllers.Api;

/// <summary>
/// API controller for order management operations.
/// 
/// Handles order creation, retrieval, and listing.
/// All endpoints require authentication.
/// Customers can view their own orders; admin operations may have additional permissions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrdersService ordersService) : ControllerBase
{
    /// <summary>
    /// Retrieve all orders (currently returns all orders, should be filtered by user in production).
    /// 
    /// Requires authentication.
    /// </summary>
    /// <returns>A collection of OrderResponseDto objects representing all orders.</returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersAsync()
    {
        var orders = await ordersService.GetOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Create a new order (customer only).
    /// 
    /// Requires CustomerPolicy authorization.
    /// The order is associated with the authenticated user's ID.
    /// All services in the order must be available and exist in the system.
    /// </summary>
    /// <param name="orderDto">Contains the list of order items (services and quantities).</param>
    /// <returns>
    /// Returns 201 Created with the new OrderResponseDto and Location header.
    /// Returns 400 Bad Request if order validation fails (unavailable services, etc.).
    /// </returns>
    [HttpPost]
    [Authorize(Policy = "CustomerPolicy")]
    public async Task<ActionResult<Order>> CreateOrderAsync(OrderDto orderDto)
    {
        var newOrder = await ordersService.CreateOrderAsync(orderDto);

        return StatusCode(201, newOrder);
    }
}
