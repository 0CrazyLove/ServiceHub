using Backend.Models;
using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Backend.Services.Implementations;

public class AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : IAuthService
{
    public async Task<(User? response, bool succeeded)> RegisterUserAsync(RegisterDto model)
    {
        var user = new IdentityUser
        {
            UserName = model.UserName,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded) return (null, false);


        await userManager.AddToRoleAsync(user, "Customer");

        var roles = await userManager.GetRolesAsync(user);

        var token = GenerateJwtToken(user, roles);

        var response = new User
        {
            Token = token,
            Username = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };
        return (response, true);
    }
    public async Task<(User? response, bool succeeded)> LoginUserAsync(LoginDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
        {
            await Task.Delay(Random.Shared.Next(100, 300));

            return (null, false);
        }

        var user = await userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            await Task.Delay(Random.Shared.Next(100, 300));

            return (null, false);
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {

            await Task.Delay(Random.Shared.Next(100, 300));

            return (null, false);
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        var response = new User
        {
            Token = token,
            Username = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };

        return (response, true);

    }


    private static string GenerateJwtToken(IdentityUser user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.UserName!)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES")!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
