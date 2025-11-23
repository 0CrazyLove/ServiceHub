/// <summary>
/// ServiceHub Backend Application Entry Point
/// 
/// Clean and organized configuration using extension methods for:
/// - Database and Identity setup
/// - JWT and Google OAuth authentication
/// - Authorization policies
/// - CORS configuration
/// - Business services registration
/// - Database seeding
/// </summary>

using Backend.Extensions;
using Backend.Services.Database.Interfaces;
using DotNetEnv;
using System.Diagnostics;
using Backend.Configurations;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

var jwtSettings = new JwtSettings();

// Configure all services using extension methods
builder.Services.AddDatabaseServices();
builder.Services.AddJwtAuthentication(jwtSettings);
builder.Services.AddAuthorizationPolicies();
builder.Services.AddGoogleOAuth();
builder.Services.AddCorsConfiguration();
builder.Services.AddApplicationServices();

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Seed database on startup
await SeedDatabaseAsync(app);

// Configure middleware pipeline
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Seeds the database using the DatabaseSeeder service.
/// </summary>
static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedAsync();
}