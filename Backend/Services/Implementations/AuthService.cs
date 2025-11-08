using Backend.Models;
using Backend.Models.DTOs;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations;

public class AuthService : IAuthService
{
    // Implement logic for JWT creation and user management
    public Task<string> Login(LoginDto loginDto)
    {
        // Dummy implementation
        return Task.FromResult("dummy-jwt-token");
    }

    public Task<User> Register(User user, string password)
    {
        // Dummy implementation
        return Task.FromResult(user);
    }
}
