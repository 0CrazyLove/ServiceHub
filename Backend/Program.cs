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
using DotNetEnv;
using System.Diagnostics;
using Backend.Configurations;
using Backend.Data;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

var jwtSettings = new JwtSettings();
var corsSettings = new CorsSettings();
var environment = builder.Environment;

// Configure all services using extension methods
builder.Services.AddDatabaseServices();
builder.Services.AddJwtAuthentication(jwtSettings);
builder.Services.AddAuthorizationPolicies();
builder.Services.AddGoogleOAuth();
builder.Services.AddCorsConfiguration(corsSettings, environment);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Seed database on startup
await app.ApplyMigrations<AppDbContext>();
await app.SeedDatabaseAsync();

// Configure middleware pipeline
app.UseGlobalMiddleware();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthEndpoints();

app.Run();