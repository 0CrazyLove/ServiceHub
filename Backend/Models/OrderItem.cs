using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ServiceId { get; set; }
    public Service? Service { get; set; }
    public int Quantity { get; set; }
    [Precision(18,2)]
    public decimal Price { get; set; }
}
