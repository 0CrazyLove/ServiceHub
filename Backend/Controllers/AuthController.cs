using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var (response, succeeded) = await authService.RegisterUserAsync(model);

        if (!succeeded) return BadRequest();

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var (response, succeeded) = await authService.LoginUserAsync(model);

        if (!succeeded) return Unauthorized();

        return Ok(response);
    }
}
