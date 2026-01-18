/// <summary>
/// Implementation of the <see cref="IDatabaseSeeder"/> service.
/// Responsible for initializing the identity system with required roles (Admin, Customer)
/// and ensuring the existence of a default administrator account.
/// </summary>
using Microsoft.AspNetCore.Identity;
using Backend.Services.Database.Interfaces;
using System.Diagnostics;
using Backend.Configurations;

namespace Backend.Services.Database.Implementations;

public class DatabaseSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<DatabaseSeeder> logger,
 IHttpContextAccessor httpContextAccessor, AdminCredentials adminCredentials) : IDatabaseSeeder
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
            var adminEmail = adminCredentials.Email;
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                logger.LogInformation("Created role Admin. CorrelationId: {CorrelationId}", correlationId);
            }

            if (adminUser is null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminCredentials.Username,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    NormalizedEmail = adminEmail.ToUpperInvariant(),
                    NormalizedUserName = adminCredentials.Username.ToUpperInvariant()
                };

                await userManager.CreateAsync(adminUser, adminCredentials.Password);
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Created admin user: {Email}. CorrelationId: {CorrelationId}", adminEmail, correlationId);
            }
            else
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Added existing admin user to Admin role. CorrelationId: {CorrelationId}", correlationId);
                }
                var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                var result = await userManager.ResetPasswordAsync(adminUser, token, adminCredentials.Password);

                if (result.Succeeded)
                {
                    logger.LogInformation("Updated admin password. CorrelationId: {CorrelationId}", correlationId);
                }
                else
                {
                    logger.LogWarning("Failed to update admin password: {Errors}. CorrelationId: {CorrelationId}",
                    string.Join(", ", result.Errors.Select(e => e.Description)), correlationId);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding admin user. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }
}