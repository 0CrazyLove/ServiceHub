/// <summary>
/// Extension methods for configuring CORS (Cross-Origin Resource Sharing) policies.
/// </summary>
/// <remarks>
/// This class provides extension methods to configure CORS settings for the application,
/// supporting different origins based on the deployment environment (Development or Production).
/// </remarks>

namespace Backend.Extensions;

using Backend.Configurations;

public static class CorsExtensions
{
    /// <summary>
    /// Configures CORS policy to allow requests from the frontend application based on the environment.
    /// </summary>
    /// <remarks>
    /// In Production: Uses allowed origins from CorsSettings configuration
    /// In Development: Allows requests from http://localhost:4321 and https://localhost:4321
    /// 
    /// All requests allow any HTTP method, any header, and credentials.
    /// </remarks>
    /// <param name="services">The service collection to add CORS policy configuration to.</param>
    /// <param name="corsSettings">The CORS settings containing allowed origins for production environment.</param>
    /// <exception cref="InvalidOperationException">Thrown when AllowedOrigins is not configured in production.</exception>
    public static void AddCorsConfiguration(this IServiceCollection services, CorsSettings corsSettings, IWebHostEnvironment env)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (env.IsDevelopment())
                {
                    policy.WithOrigins("http://localhost:4321", "https://localhost:4321").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                }
                else
                {
                    if (corsSettings.AllowedOrigins.Length <= 0)
                    {
                        throw new InvalidOperationException("CORS_ALLOWED_ORIGINS no está configurado en producción");
                    }
                    policy.WithOrigins(corsSettings.AllowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                }
            });
        });

    }
}