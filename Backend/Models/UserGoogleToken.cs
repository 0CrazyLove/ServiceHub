namespace Backend.Models;

/// <summary>
/// Stores Google OAuth refresh tokens for users authenticated via Google Sign-In.
/// 
/// This model maintains the relationship between users and their Google refresh tokens,
/// allowing the application to maintain long-lived sessions and perform background tasks
/// on behalf of the user without requiring them to re-authenticate.
/// </summary>
public class UserGoogleToken
{
    /// <summary>
    /// Gets or sets the unique identifier for this token record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID this token belongs to.
    /// References the AspNetUsers table.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Google refresh token.
    /// This token is used to obtain new access tokens when the current one expires.
    /// Must be stored securely and never exposed to the client.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when the refresh token expires.
    /// Google refresh tokens typically have very long expiration times (years).
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when this record was created.
    /// Used for audit purposes and token lifecycle management.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}