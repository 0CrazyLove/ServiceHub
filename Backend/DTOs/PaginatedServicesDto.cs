namespace Backend.DTOs;

public class PaginatedServicesDto
{
    public List<ServiceResponseDto> Items { get; set; } = [];
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
