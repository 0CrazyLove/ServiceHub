/// <summary>
/// Database seeding service implementation.
/// Creates default roles and admin user on application startup.
/// </summary>

using Microsoft.AspNetCore.Identity;
using Backend.Services.Database.Interfaces;
namespace Backend.Services.Database.Implementations;

public class DatabaseSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : IDatabaseSeeder
{

    /// <summary>
    /// Seeds the database with Admin and Customer roles, and creates default admin user.
    /// Default credentials: username "admin", email "admin@example.com", password "Admin123!"
    /// </summary>
    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
    }

    private async Task SeedRolesAsync()
    {
        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create Customer role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new IdentityRole("Customer"));
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        // Create default admin user if not already present
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
        }
    }
}