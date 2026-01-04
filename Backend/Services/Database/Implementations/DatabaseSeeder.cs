/// <summary>
/// Implementation of the <see cref="IDatabaseSeeder"/> service.
/// Responsible for initializing the identity system with required roles (Admin, Customer)
/// and ensuring the existence of a default administrator account.
/// </summary>
using Microsoft.AspNetCore.Identity;
using Backend.Services.Database.Interfaces;
using System.Diagnostics;

namespace Backend.Services.Database.Implementations;

public class DatabaseSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<DatabaseSeeder> logger, IHttpContextAccessor httpContextAccessor) : IDatabaseSeeder
{
    /// <inheritdoc />
    public async Task SeedAsync()
    {
        // Genera un correlationId si no existe ninguno
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

        logger.LogDebug("Starting database seeding. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            await SeedRolesAsync(correlationId);
            await SeedAdminUserAsync(correlationId);
            logger.LogInformation("Database seeding completed successfully. CorrelationId: {CorrelationId}", correlationId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database seeding. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    private async Task SeedRolesAsync(string correlationId)
    {
        logger.LogDebug("Seeding roles. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            string[] allRoles = ["Admin", "Customer"];

            foreach (var role in allRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Created role: {Role}. CorrelationId: {CorrelationId}", role, correlationId);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding roles. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    private async Task SeedAdminUserAsync(string correlationId)
    {
        logger.LogDebug("Seeding admin user. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser is null)
            {
                adminUser = new IdentityUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Created admin user: {Email}. CorrelationId: {CorrelationId}", adminEmail, correlationId);
            }
            else
            {
                logger.LogDebug("Admin user already exists. CorrelationId: {CorrelationId}", correlationId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding admin user. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }
}