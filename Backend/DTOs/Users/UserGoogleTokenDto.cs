namespace Backend.DTOs.Users;

/// <summary>
/// Data Transfer Object for retrieving Google token information.
/// 
/// Contains basic information about a user's stored Google refresh token.
/// Used for reading token metadata without exposing sensitive data.
/// </summary>
public class UserGoogleTokenDto
{
    /// <summary>
    /// Gets or sets the user ID this token belongs to.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Google refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when the token expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Data Transfer Object for creating Google token records.
/// 
/// Sent when storing a new Google refresh token for a user.
/// The expiration time is specified in seconds.
/// </summary>
public class CreateUserGoogleTokenDto
{
    /// <summary>
    /// Gets or sets the user ID to associate with the token.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Google refresh token to store.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiration time in seconds.
    /// Used to calculate the absolute expiration timestamp (ExpiresAt).
    /// </summary>
    public int ExpiresIn { get; set; }
}

/// <summary>
/// Data Transfer Object for updating Google token records.
/// 
/// Sent when refreshing or updating an existing token for a user.
/// </summary>
public class UpdateUserGoogleTokenDto
{
    /// <summary>
    /// Gets or sets the new Google refresh token value.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiration time in seconds from now.
    /// Used to calculate the new absolute expiration timestamp.
    /// </summary>
    public int ExpiresIn { get; set; }
}