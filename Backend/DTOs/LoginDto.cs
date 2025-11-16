namespace Backend.DTOs;

/// <summary>
/// Data Transfer Object for user login requests.
/// 
/// Contains the credentials required to authenticate a user
/// via email and password.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Gets or sets the user's email address for authentication.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the user's password for authentication.
    /// Must meet the configured password policy requirements.
    /// </summary>
    public string? Password { get; set; }
}
