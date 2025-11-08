using Backend.Models;
using Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private static List<Order> _orders = new List<Order>(); // Dummy in-memory store

        [HttpGet]
        // [Authorize]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            // In a real app, you'd filter by user
            return Ok(_orders);
        }

        [HttpPost]
        // [Authorize(Roles = "Customer")]
        public ActionResult<Order> CreateOrder(OrderDto orderDto)
        {
            // Dummy implementation
            var newOrder = new Order
            {
                Id = _orders.Any() ? _orders.Max(o => o.Id) + 1 : 1,
                UserId = orderDto.UserId,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>(),
                TotalAmount = 0 // Calculate this based on products
            };

            // In a real app, you would:
            // 1. Get products from DB by IDs in orderDto.OrderItems
            // 2. Calculate total amount
            // 3. Decrement product stock
            // 4. Save the order

            _orders.Add(newOrder);

            return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, newOrder);
        }

        [HttpGet("{id}")]
        // [Authorize]
        public ActionResult<Order> GetOrder(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
    }
