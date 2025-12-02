/// <summary>
/// Database configuration extensions for dependency injection.
/// Configures Entity Framework Core and Identity services.
/// </summary>

using Backend.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// Configures the database context and ASP.NET Core Identity services.
    /// </summary>
    /// <param name="services">The service collection to add database services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        // Configure Entity Framework Core with SQL Server
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING")));

        // Configure ASP.NET Core Identity
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}