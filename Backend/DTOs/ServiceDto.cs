using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class ServiceDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    [Precision(18,2)]
    public decimal Price { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string PriceType { get; set; } = "proyecto";
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    [Required(ErrorMessage = "El proveedor es requerido")]
    [MaxLength(200)]
    public string Provider { get; set; } = string.Empty;
    
    [Range(0, 5)]
    [Precision(3,2)]
    public decimal Rating { get; set; } = 0;
    
    [Range(0, int.MaxValue)]
    public int ReviewCount { get; set; } = 0;
    
    [Range(0, int.MaxValue)]
    public int CompletedJobs { get; set; } = 0;
    
    [Required]
    [MaxLength(100)]
    public string DeliveryTime { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    public bool Verified { get; set; } = false;
    
    public bool Available { get; set; } = true;
    
    // Lista de idiomas
    public List<string> Languages { get; set; } = new();
}
