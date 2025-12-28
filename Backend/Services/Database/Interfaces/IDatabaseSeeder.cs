/// <summary>
/// Defines the contract for the database seeding service.
/// This service is responsible for populating the database with initial structural data,
/// such as default roles and administrative accounts, ensuring the application starts in a valid state.
/// </summary>
namespace Backend.Services.Database.Interfaces;

public interface IDatabaseSeeder
{
    /// <summary>
    /// Asynchronously executes the database seeding process.
    /// </summary>
    /// <remarks>
    /// This method should be idempotent, meaning it can be safely called multiple times without
    /// causing data duplication or inconsistencies. It is typically invoked during the application startup sequence.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SeedAsync();
}