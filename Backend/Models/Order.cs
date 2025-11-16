using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

/// <summary>
/// Represents a customer order in the ServiceHub platform.
/// 
/// An order contains one or more order items (services) and tracks the total amount,
/// order date, and associated customer. Orders are immutable once created and serve
/// as the primary transaction record for the platform.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID associated with this order.
    /// References the AspNetUsers table through Identity framework.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the customer (IdentityUser).
    /// </summary>
    public IdentityUser? User { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the order was created (UTC).
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the order in the platform's currency.
    /// Calculated as the sum of all order items (quantity × price).
    /// </summary>
    [Precision(18, 2)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the collection of order items (services) included in this order.
    /// </summary>
    public IList<OrderItem> OrderItems { get; set; } = [];
}
