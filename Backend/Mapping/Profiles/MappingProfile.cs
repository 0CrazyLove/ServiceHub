using AutoMapper;
using Backend.DTOs.Orders;
using Backend.DTOs.Services;
using Backend.Models;
using Backend.Mapping.Converters;

namespace Backend.Mapping.Profiles;

/// <summary>
/// AutoMapper profile configuration for the application.
/// Defines mapping rules between Entities and DTOs.
/// Configures custom type converters for complex types like JSON strings.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Service, ServiceResponseDto>().ForMember(dest => dest.Languages, opt => opt.ConvertUsing(new JsonStringToStringListConverter()));

        CreateMap<ServiceDto, Service>().ForMember(dest => dest.Languages, opt => opt.ConvertUsing(new StringListToJsonStringConverter()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow)).ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Order, OrderResponseDto>();

        CreateMap<OrderItem, OrderItemResponseDto>().ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : null));
    }
}
