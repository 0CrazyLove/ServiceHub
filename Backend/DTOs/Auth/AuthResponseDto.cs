namespace Backend.DTOs.Auth;

/// <summary>
/// Data Transfer Object for authentication responses.
/// 
/// Returned after successful user login or registration, containing
/// the JWT authentication token and user information. This DTO does not
/// contain sensitive information and is safe to return to the client.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Gets or sets the JWT bearer token for subsequent API requests.
    /// The client must include this token in the Authorization header
    /// as "Bearer {token}" for authenticated requests.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username of the authenticated user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the authenticated user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of roles assigned to the user.
    /// Examples: ["Customer", "Admin", "Vendor"]
    /// Used for authorization decisions in the frontend.
    /// </summary>
    public IList<string> Roles { get; set; } = [];
}
