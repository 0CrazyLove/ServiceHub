using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

/// <summary>
/// Represents a single service item within an order.
/// 
/// OrderItem is a join entity that links a Service to an Order, allowing
/// customers to purchase multiple services in a single order transaction.
/// Each item tracks the quantity, price at purchase time, and quantity purchased.
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the order item.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key reference to the parent Order.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the parent Order.
    /// </summary>
    public Order? Order { get; set; }

    /// <summary>
    /// Gets or sets the foreign key reference to the Service being ordered.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the Service.
    /// </summary>
    public Service? Service { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the service ordered.
    /// For services with unit-based pricing.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the service at the time of purchase.
    /// This is stored separately to preserve historical pricing information
    /// even if the Service record is updated later.
    /// </summary>
    [Precision(18, 2)]
    public decimal Price { get; set; }
}
