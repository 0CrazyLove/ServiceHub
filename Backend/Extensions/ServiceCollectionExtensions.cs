/// <summary>
/// Business services dependency injection extensions.
/// Registers all application services and configurations.
/// </summary>

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

namespace Backend.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all business services for dependency injection.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
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

        // Configuration settings
        services.AddSingleton<JwtSettings>();
        services.AddSingleton<GoogleSettings>();

        return services;
    }
}