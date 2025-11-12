namespace Backend.DTOs;

public class ServiceResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string PriceType { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string Provider { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public int CompletedJobs { get; set; }
    public string DeliveryTime { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool Verified { get; set; }
    public bool Available { get; set; }
    public List<string> Languages { get; set; } =[];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}