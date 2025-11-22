using Backend.DTOs.Auth.GoogleAuth;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services.Auth.Interfaces;

/// <summary>
/// Service for handling Google OAuth operations.
/// </summary>
public interface IGoogleAuthService
{
    /// <summary>
    /// Exchange Google authorization code for access and refresh tokens.
    /// </summary>
    /// <param name="code">Authorization code from Google</param>
    /// <returns>Google token response containing access token, refresh token, and expiration</returns>
    Task<GoogleTokenDto> ExchangeCodeForTokensAsync(string code);

    /// <summary>
    /// Decode and validate Google ID token.
    /// </summary>
    /// <param name="idToken">Google ID token (JWT)</param>
    /// <returns>Validated user information from the token</returns>
    Task<GoogleJwtPayloadDto> DecodeAndValidateIdTokenAsync(string idToken);
    /// <summary>
    /// Find or create a user from Google authentication.
    /// </summary>
    /// <param name="userInfo">Google user information</param>
    /// <returns>Identity user and operation success status</returns>
    Task<(IdentityUser? user, bool succeeded)> FindOrCreateGoogleUserAsync(GoogleJwtPayloadDto userInfo);

    /// <summary>
    /// Update user's Google-related claims.
    /// </summary>
    /// <param name="user">The identity user</param>
    /// <param name="userInfo">Google user information</param>
    Task UpdateGoogleClaimsAsync(IdentityUser user, GoogleJwtPayloadDto userInfo);
}