namespace Backend.Configurations;

/// <summary>
/// Represents administrator account credentials.
/// </summary>
public class AdminCredentials
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminCredentials"/> class
    /// with values from environment variables.
    /// </summary>
    public AdminCredentials()
    {
        Email = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? string.Empty;
        Password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? string.Empty;
        Username = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the administrator email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the administrator password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the administrator username.
    /// </summary>
    public string Username { get; set; } = string.Empty;
}