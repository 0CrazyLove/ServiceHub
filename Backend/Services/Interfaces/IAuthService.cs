using Backend.DTOs;
namespace Backend.Services.Interfaces;

public interface IAuthService
{
    Task<(AuthResponseDto? response, bool succeeded)> RegisterUserAsync(RegisterDto model);
    Task<(AuthResponseDto? response, bool succeeded)> LoginUserAsync(LoginDto model);
    Task SaveRefreshTokenAsync(string userId, string refreshToken, int expiresIn);
    Task<(AuthResponseDto? response, bool succeeded)> GoogleCallbackAsync(string code);

}