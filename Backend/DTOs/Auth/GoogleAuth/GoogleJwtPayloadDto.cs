using System.Text.Json.Serialization;
namespace Backend.DTOs.Auth.GoogleAuth;
/// <summary>
/// Google OAuth JWT payload model.
/// 
/// Represents the user information returned by Google's userinfo endpoint.
/// Uses JsonPropertyName attributes for proper deserialization of Google's response format.
/// </summary>
public class GoogleJwtPayloadDto
{
    /// <summary>
    /// The unique Google user ID (subject).
    /// </summary>
    [JsonPropertyName("sub")]
    public string Sub { get; set; } = string.Empty;

    /// <summary>
    /// The user's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Whether the email address has been verified by Google.
    /// </summary>
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    /// <summary>
    /// The user's full name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The URL to the user's profile picture.
    /// </summary>
    [JsonPropertyName("picture")]
    public string? Picture { get; set; }
}