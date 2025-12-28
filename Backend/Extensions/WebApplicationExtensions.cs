using Backend.Services.Database.Interfaces;
namespace Backend.Extensions;

/// <summary>
/// Application startup helpers for <see cref="WebApplication"/>.
/// </summary>
/// <remarks>
/// Designed to be invoked during startup to perform host-level tasks
/// that require a scoped service provider (for example, seeding data).
/// </remarks>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Creates a scoped service provider and executes the registered
    /// <see cref="IDatabaseSeeder"/> to populate initial or required data.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> used to resolve services.</param>
    /// <returns>A <see cref="Task"/> that completes when the seeding operation finishes.</returns>
    /// <remarks>
    /// Call this during application startup after building the host and before
    /// the application begins accepting requests.
    /// </remarks>
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        
        await seeder.SeedAsync();
    }
}