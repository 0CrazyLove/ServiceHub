namespace Backend.DTOs;

/// <summary>
/// Model representing the authentication response returned after successful login or registration.
/// Contains the JWT token and user information.
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;

    public IList<string> Roles { get; set; } = [];
}
