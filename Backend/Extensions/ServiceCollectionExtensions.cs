using Backend.Configurations;
using Backend.Services.Auth.Implementations;
using Backend.Services.Auth.Interfaces;
using Backend.Services.Business.Implemetations;
using Backend.Services.Business.Interfaces;
using Backend.Services.Dashboard.Implementations;
using Backend.Services.Dashboard.Interfaces;
using Backend.Services.Orders.Implementations;
using Backend.Services.Orders.Interfaces;
using Backend.Services.Database.Implementations;
using Backend.Services.Database.Interfaces;
using Backend.Repository.Interfaces;
using Backend.Repository.Implementations;

namespace Backend.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all business services and configurations for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to add application services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Infrastructure/Framework services
        services.AddHttpContextAccessor();

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Auth services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // Business services
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IOrdersService, OrdersService>();
        services.AddScoped<IServicesService, ServicesService>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        // AutoMapper
        services.AddAutoMapper(cfg => {}, typeof(Program));

        // Configuration settings
        services.AddSingleton<JwtSettings>();
        services.AddSingleton<GoogleSettings>();
        
        return services;
    }
}