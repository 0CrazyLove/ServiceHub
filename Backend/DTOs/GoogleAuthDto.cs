namespace Backend.DTOs;

/// <summary>
/// Data Transfer Object for Google OAuth authorization code callback.
/// 
/// Sent by the client to the backend after the user completes
/// Google sign-in. Contains the authorization code that will be
/// exchanged for access and refresh tokens.
/// </summary>
public class GoogleAuthCodeDto
{
    /// <summary>
    /// Gets or sets the authorization code returned by Google OAuth.
    /// This code is exchanged on the backend for access and refresh tokens.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}
