using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Backend.Data;

/// <summary>
/// Application database context for the ServiceHub platform.
/// 
/// Extends IdentityDbContext to provide identity-related tables and functionality
/// out of the box. Includes DbSets for all domain models: Services, Orders,
/// OrderItems, and UserGoogleTokens. Manages all database interactions and
/// entity relationships through Entity Framework Core.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    /// <summary>
    /// Gets or sets the Services DbSet.
    /// Represents all services offered in the marketplace.
    /// </summary>
    public DbSet<Service> Services { get; set; }

    /// <summary>
    /// Gets or sets the Orders DbSet.
    /// Represents all customer orders placed in the system.
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Gets or sets the OrderItems DbSet.
    /// Represents individual items within orders (join entity).
    /// </summary>
    public DbSet<OrderItem> OrderItems { get; set; }

    /// <summary>
    /// Gets or sets the UserRefreshTokens DbSet.
    /// Represents stored refresh tokens for users.
    /// </summary>
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
}
