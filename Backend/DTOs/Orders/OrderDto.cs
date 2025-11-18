namespace Backend.DTOs.Orders;

/// <summary>
/// Data Transfer Object for order creation requests (input).
/// 
/// Contains a collection of order items that the customer
/// wants to purchase in a single transaction.
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Gets or sets the collection of order items.
    /// Each item specifies a service ID and quantity to purchase.
    /// </summary>
    public IList<OrderItemDto> OrderItems { get; set; } = [];
}

/// <summary>
/// Data Transfer Object for individual order item input.
/// 
/// Specifies a service and quantity within an order request.
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Gets or sets the ID of the service being ordered.
    /// Must reference an existing, available service.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the service to purchase.
    /// Must be a positive integer.
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// Data Transfer Object for order responses (output).
/// 
/// Returned after order creation or retrieval, containing
/// complete order information including all line items.
/// </summary>
public class OrderResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the order was placed.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the order.
    /// Calculated as the sum of all line items.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the collection of items in this order.
    /// </summary>
    public List<OrderItemResponseDto> OrderItems { get; set; } = [];
}

/// <summary>
/// Data Transfer Object for individual order item responses (output).
/// 
/// Represents a service purchased as part of an order, including
/// the service information and purchase details.
/// </summary>
public class OrderItemResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the order item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the service in this order item.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the name of the service at the time of purchase.
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// Gets or sets the quantity purchased.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the service at purchase time.
    /// Stored to preserve historical pricing information.
    /// </summary>
    public decimal Price { get; set; }
}