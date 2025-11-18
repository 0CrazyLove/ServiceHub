using System.Text.Json.Serialization;
namespace Backend.DTOs.Auth.GoogleAuth;
/// <summary>
/// Google OAuth token response model.
/// 
/// Represents tokens returned by Google's token endpoint.
/// Includes access token for API calls and optional refresh token for long-lived sessions.
/// </summary>
public class GoogleTokenResponseDto
{
    /// <summary>
    /// The access token for making authenticated requests to Google APIs.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The refresh token for obtaining new access tokens (optional).
    /// Only provided on first authorization or when offline_access is requested.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// The lifetime of the access token in seconds.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}