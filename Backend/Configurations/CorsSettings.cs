namespace Backend.Configurations;

/// <summary>
/// Configuration for CORS allowed origins loaded from environment variables.
/// </summary>
/// <remarks>
/// Reads comma-separated origins from the "CORS_ALLOWED_ORIGINS" environment variable.
/// Example: CORS_ALLOWED_ORIGINS=https://example.com,https://app.example.com
/// </remarks>
public class CorsSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CorsSettings"/> class.
    /// </summary>
    /// <remarks>
    /// Parses the "CORS_ALLOWED_ORIGINS" environment variable into an array of trimmed origins.
    /// </remarks>
    public CorsSettings()
    {
        var originsString = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");

        if (string.IsNullOrWhiteSpace(originsString))
        {
            AllowedOrigins = [];
        }
        else
        {
            AllowedOrigins = [.. originsString.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(origin => origin.Trim())];
        }
    }

    /// <summary>
    /// Gets or sets the array of allowed CORS origins.
    /// </summary>
    public string[] AllowedOrigins { get; set; }
}