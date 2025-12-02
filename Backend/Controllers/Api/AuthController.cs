using Backend.DTOs.Auth;
using Backend.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Api;

/// <summary>
/// API controller for authentication operations.
/// 
/// Handles user registration, login, and Google OAuth authentication.
/// All endpoints accept and return JSON formatted data.
/// Provides both traditional email/password authentication and
/// Google Sign-In capabilities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Register a new user account with email and password.
    /// </summary>
    /// <param name="model">Contains username, email, and password.</param>
    /// <returns>
    /// Returns 200 OK with AuthResponseDto containing JWT token if successful.
    /// Returns 400 Bad Request if registration fails (duplicate email/username, invalid password, etc.).
    /// </returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var (response, succeeded, errors) = await authService.RegisterUserAsync(model);

        if (!succeeded || response == null)
        {
            return BadRequest(new { errors = errors!.Select(e => e.Description) });
        }

        return Ok(response);
    }

    /// <summary>
    /// Authenticate a user with email and password credentials.
    /// </summary>
    /// <param name="model">Contains email and password.</param>
    /// <returns>
    /// Returns 200 OK with AuthResponseDto containing JWT token if credentials are valid.
    /// Returns 401 Unauthorized if credentials are invalid or user not found.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var (response, succeeded) = await authService.LoginUserAsync(model);

        if (!succeeded || response == null) return Unauthorized(new {message = "Invalid credentials"});

        return Ok(response);
    }

    /// <summary>
    /// Handle Google OAuth callback after user authorization.
    /// 
    /// Exchanges the authorization code from Google for tokens,
    /// retrieves user information, and creates or updates the user account.
    /// </summary>
    /// <param name="model">Contains the authorization code from Google.</param>
    /// <returns>
    /// Returns 200 OK with AuthResponseDto containing JWT token if successful.
    /// Returns 400 Bad Request if authorization code is missing.
    /// Returns 401 Unauthorized if Google authentication fails.
    /// </returns>
    [HttpPost("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromBody] GoogleAuthCodeDto model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(model.Code))
        {
            return BadRequest(new { message = "Authorization code is required" });
        }

        var (response, succeeded) = await authService.GoogleCallbackAsync(model.Code, cancellationToken);

        if (!succeeded || response == null)
        {
            return Unauthorized(new { message = "Google authentication failed" });
        }

        return Ok(response);
    }
}
