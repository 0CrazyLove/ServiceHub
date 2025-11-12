using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

public class Service
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Precision(18,2)]
    public decimal Price { get; set; }
    
    [MaxLength(50)]
    public string PriceType { get; set; } = "proyecto"; // proyecto, hora, mes, etc.
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    [MaxLength(200)]
    public string Provider { get; set; } = string.Empty;
    
    [Precision(3,2)]
    public decimal Rating { get; set; } = 0;
    
    public int ReviewCount { get; set; } = 0;
    
    public int CompletedJobs { get; set; } = 0;
    
    [MaxLength(100)]
    public string DeliveryTime { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    public bool Verified { get; set; } = false;
    
    public bool Available { get; set; } = true;
    
    // Almacenar idiomas como JSON string: ["Español","Inglés"]
    [MaxLength(500)]
    public string Languages { get; set; } = "[]";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}