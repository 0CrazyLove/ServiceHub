using Microsoft.AspNetCore.Authorization;
using Backend.Models;
using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;
using System.Security.Claims;

namespace Backend.Controllers;

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
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        var orders = await ordersService.GetOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Retrieve a specific order by ID.
    /// 
    /// Requires authentication.
    /// In production, should verify that the requesting user owns the order.
    /// </summary>
    /// <param name="id">The order ID to retrieve.</param>
    /// <returns>
    /// Returns 200 OK with OrderResponseDto if found.
    /// Returns 404 Not Found if order does not exist.
    /// </returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await ordersService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
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
    public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var newOrder = await ordersService.CreateOrderAsync(orderDto, userId);
        return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, newOrder);
    }
}
