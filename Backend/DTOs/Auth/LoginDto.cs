using System.ComponentModel.DataAnnotations;
namespace Backend.DTOs.Auth;

public class LoginDto
{
    /// <summary>
    /// Gets or sets the user's email address for authentication.
    /// </summary>
    [Required]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the user's password for authentication.
    /// Must meet the configured password policy requirements.
    /// </summary>
    [Required]
    public string? Password { get; set; }
}