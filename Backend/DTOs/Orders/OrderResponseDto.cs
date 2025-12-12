namespace Backend.DTOs.Orders;

/// <summary>
/// Response DTO for a complete order.
/// </summary>
public class OrderResponseDto
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID associated with the order.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the order was placed.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the total order amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the collection of order items.
    /// </summary>
    public List<OrderItemResponseDto> OrderItems { get; set; } = [];
}