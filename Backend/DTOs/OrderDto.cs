namespace Backend.DTOs;

// DTO para CREAR orden (input)
public class OrderDto
{
    public IList<OrderItemDto> OrderItems { get; set; } = [];
}

public class OrderItemDto
{
    public int ServiceId { get; set; }
    public int Quantity { get; set; }
}

// DTO para RESPUESTA (output)
public class OrderResponseDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemResponseDto> OrderItems { get; set; } = [];
}

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}