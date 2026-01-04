using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Auth;

/// <summary>
/// Data Transfer Object for refresh token requests.
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// The refresh token string.
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
