namespace Backend.DTOs;
public class OrderDto
{
    public int UserId { get; set; }
    public IList<OrderItemDto> OrderItems { get; set; } = [];
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
