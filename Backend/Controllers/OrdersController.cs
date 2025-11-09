using Microsoft.AspNetCore.Authorization;
using Backend.Models;
using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrdersService ordersService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        // In a real app, you'd filter by user
        var orders = await ordersService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
    {
        var newOrder = await ordersService.CreateOrderAsync(orderDto);
        return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, newOrder);
    }

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
}
