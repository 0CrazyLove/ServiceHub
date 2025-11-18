using System.ComponentModel.DataAnnotations;
namespace Backend.DTOs.Auth;
public class RegisterDto
{
    /// <summary>
    /// Gets or sets the username for the new account.
    /// Must be unique and required.
    /// </summary>
    [Required]
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the email address for the new account.
    /// Must be unique and required.
    /// </summary>
    [Required]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password for the new account.
    /// Must meet the configured password policy:
    /// - At least 6 characters
    /// - Contains at least one uppercase letter
    /// - Contains at least one lowercase letter
    /// - Contains at least one digit
    /// </summary>
    [Required]
    public string? Password { get; set; }
}