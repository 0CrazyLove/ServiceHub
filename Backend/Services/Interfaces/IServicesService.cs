using Backend.Models;
using Backend.DTOs;

namespace Backend.Services.Interfaces;

public interface IServicesService
{
    Task<PaginatedServicesDto> GetServices(string? category, int page, int pageSize, decimal? minPrice, decimal? maxPrice);
    Task<ServiceResponseDto?> GetServiceById(int id);
    Task<ServiceResponseDto> CreateService(ServiceDto serviceDto);
    Task<ServiceResponseDto?> UpdateService(int id, ServiceDto serviceDto);
    Task<bool> DeleteService(int id);
    Task<IEnumerable<string>> GetCategories();
}
