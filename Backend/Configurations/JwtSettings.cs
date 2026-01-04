namespace Backend.Configurations;

/// <summary>
/// Configuration class for JWT (JSON Web Token) settings.
/// Reads JWT configuration values from environment variables.
/// </summary>
public class JwtSettings
{
    public JwtSettings()
    {
        SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!;
        Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
        Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
        ExpirationMinutes = Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES"))!;
        RefreshTokenExpiryDays = 7;
    }
    /// <summary>
    /// Gets or sets the secret key used to sign and verify JWT tokens.
    /// Retrieved from the JWT_SECRET_KEY environment variable.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the issuer of the JWT token.
    /// Retrieved from the JWT_ISSUER environment variable.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience for which the JWT token is intended.
    /// Retrieved from the JWT_AUDIENCE environment variable.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration time for JWT tokens in minutes.
    /// Retrieved from the JWT_EXPIRATION_MINUTES environment variable.
    /// </summary>
    public int ExpirationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for refresh tokens in days.
    /// </summary>
    public int RefreshTokenExpiryDays { get; set; }
}
