using Backend.Models;
using Backend.DTOs;
namespace Backend.Services.Interfaces;

public interface IAuthService
{
    Task<(User? response, bool succeeded)> RegisterUserAsync(RegisterDto model);
    Task<(User? response, bool succeeded)> LoginUserAsync(LoginDto model);

}