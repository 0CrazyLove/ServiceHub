using Backend.Data;
using Backend.Models;
using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Backend.Services.Implementations;

public class ServicesService(AppDbContext context) : IServicesService
{
    public async Task<PaginatedServicesDto> GetServices(
        string? category, 
        int page, 
        int pageSize,
        decimal? minPrice,
        decimal? maxPrice)
    {
        var query = context.Services.AsQueryable();
        
        // Filtro por categoría
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(s => s.Category == category);
        }
        
        // Filtro por precio mínimo
        if (minPrice.HasValue)
        {
            query = query.Where(s => s.Price >= minPrice.Value);
        }
        
        // Filtro por precio máximo
        if (maxPrice.HasValue)
        {
            query = query.Where(s => s.Price <= maxPrice.Value);
        }
        
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        var services = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var serviceDtos = services.Select(MapToResponseDto).ToList();
        
        return new PaginatedServicesDto
        {
            Items = serviceDtos,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    public async Task<ServiceResponseDto?> GetServiceById(int id)
    {
        var service = await context.Services.FindAsync(id);
        return service == null ? null : MapToResponseDto(service);
    }
    
    public async Task<ServiceResponseDto> CreateService(ServiceDto serviceDto)
    {
        var newService = new Service
        {
            Name = serviceDto.Name,
            Description = serviceDto.Description,
            Price = serviceDto.Price,
            PriceType = serviceDto.PriceType,
            Category = serviceDto.Category,
            Provider = serviceDto.Provider,
            Rating = serviceDto.Rating,
            ReviewCount = serviceDto.ReviewCount,
            CompletedJobs = serviceDto.CompletedJobs,
            DeliveryTime = serviceDto.DeliveryTime,
            ImageUrl = serviceDto.ImageUrl,
            Verified = serviceDto.Verified,
            Available = serviceDto.Available,
            Languages = JsonSerializer.Serialize(serviceDto.Languages),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        context.Services.Add(newService);
        await context.SaveChangesAsync();
        
        return MapToResponseDto(newService);
    }
    
    public async Task<ServiceResponseDto?> UpdateService(int id, ServiceDto serviceDto)
    {
        var service = await context.Services.FindAsync(id);
        if (service == null) return null;
        
        service.Name = serviceDto.Name;
        service.Description = serviceDto.Description;
        service.Price = serviceDto.Price;
        service.PriceType = serviceDto.PriceType;
        service.Category = serviceDto.Category;
        service.Provider = serviceDto.Provider;
        service.Rating = serviceDto.Rating;
        service.ReviewCount = serviceDto.ReviewCount;
        service.CompletedJobs = serviceDto.CompletedJobs;
        service.DeliveryTime = serviceDto.DeliveryTime;
        service.ImageUrl = serviceDto.ImageUrl;
        service.Verified = serviceDto.Verified;
        service.Available = serviceDto.Available;
        service.Languages = JsonSerializer.Serialize(serviceDto.Languages);
        service.UpdatedAt = DateTime.UtcNow;
        
        await context.SaveChangesAsync();
        
        return MapToResponseDto(service);
    }
    
    public async Task<bool> DeleteService(int id)
    {
        var service = await context.Services.FindAsync(id);
        if (service == null) return false;
        
        context.Services.Remove(service);
        await context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<IEnumerable<string>> GetCategories()
    {
        return await context.Services
            .Where(s => !string.IsNullOrEmpty(s.Category))
            .Select(s => s.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
    
    private static ServiceResponseDto MapToResponseDto(Service service)
    {
        List<string> languages;
        try
        {
            languages = JsonSerializer.Deserialize<List<string>>(service.Languages) ?? new List<string>();
        }
        catch
        {
            languages = new List<string>();
        }
        
        return new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price,
            PriceType = service.PriceType,
            Category = service.Category,
            Provider = service.Provider,
            Rating = service.Rating,
            ReviewCount = service.ReviewCount,
            CompletedJobs = service.CompletedJobs,
            DeliveryTime = service.DeliveryTime,
            ImageUrl = service.ImageUrl,
            Verified = service.Verified,
            Available = service.Available,
            Languages = languages,
            CreatedAt = service.CreatedAt,
            UpdatedAt = service.UpdatedAt
        };
    }
}
