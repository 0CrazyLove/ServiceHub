using System.Collections.Generic;

namespace Backend.Models.DTOs;
    public class OrderDto
    {
        public int UserId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
