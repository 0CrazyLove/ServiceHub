using Backend.Models;
using Backend.Models.DTOs;
namespace Backend.Services.Interfaces;

public interface IAuthService
{
    Task<User> Register(User user, string password);
    Task<string> Login(LoginDto loginDto);
    // Task<bool> UserExists(string username); // This might be internal
}