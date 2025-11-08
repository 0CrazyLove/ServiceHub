using Backend.Models.DTOs;
using Backend.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginDto loginDto)
        {
            // Dummy implementation
            return Ok(new { message = "Registro exitoso" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await authService.Login(loginDto);
            return Ok(new { token });
        }
    }

