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
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
        logger.LogDebug("Starting database seeding. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
            logger.LogInformation("Database seeding completed successfully. CorrelationId: {CorrelationId}", correlationId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database seeding. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    /// <summary>
    /// Ensures that the required application roles exist in the persistence store.
    /// </summary>
    /// <remarks>
    /// This method checks for the existence of "Admin" and "Customer" roles.
    /// If a role is missing, it is created. This ensures that the authorization system
    /// has the necessary claims foundation to function correctly.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task SeedRolesAsync()
    {
        var correlationId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
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

    /// <summary>
    /// Provisions a default administrator user if one does not already exist.
    /// </summary>
    /// <remarks>
    /// The default admin user is created with the email "admin@example.com" and a predefined password.
    /// This account is automatically assigned to the "Admin" role and has its email confirmed,
    /// allowing immediate access to administrative features upon initial deployment.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task SeedAdminUserAsync()
    {
        var correlationId = Activity.Current?.Id ?? "unknown";
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