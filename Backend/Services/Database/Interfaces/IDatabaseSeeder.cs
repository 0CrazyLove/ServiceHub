/// <summary>
/// Interface for database seeding operations.
/// Used to initialize database with default data on application startup.
/// </summary>

namespace Backend.Services.Database.Interfaces;

public interface IDatabaseSeeder
{
    /// <summary>
    /// Seeds the database with initial data including roles and admin user.
    /// </summary>
    Task SeedAsync();
}