using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;



namespace Backend.Services.Implementations;

public class AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpClientFactory httpClientFactory, AppDbContext context) : IAuthService
{
    public async Task<(AuthResponseDto? response, bool succeeded)> RegisterUserAsync(RegisterDto model)
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

        var response = new AuthResponseDto
        {
            Token = token,
            Username = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };
        return (response, true);
    }
    public async Task<(AuthResponseDto? response, bool succeeded)> LoginUserAsync(LoginDto model)
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

        var response = new AuthResponseDto
        {
            Token = token,
            Username = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };

        return (response, true);

    }
    public async Task<(AuthResponseDto? response, bool succeeded)> GoogleCallbackAsync(string code)
    {
        try
        {
            // 1. Intercambiar el code por tokens con Google
            var tokenResponse = await ExchangeCodeForTokensAsync(code);

            // 2. Obtener información del usuario de Google
            var userInfo = await GetGoogleUserInfoAsync(tokenResponse.AccessToken);

            // 3. Buscar o crear usuario
            var user = await userManager.FindByEmailAsync(userInfo.Email);

            if (user == null)
            {
                // Crear nuevo usuario
                user = new IdentityUser
                {
                    UserName = userInfo.Email,
                    Email = userInfo.Email,
                    EmailConfirmed = userInfo.VerifiedEmail
                };

                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return (null, false);
                }

                // Asignar rol Customer por defecto
                await userManager.AddToRoleAsync(user, "Customer");
            }

            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                await SaveRefreshTokenAsync(user.Id, tokenResponse.RefreshToken, tokenResponse.ExpiresIn);
            }

            // 4. Actualizar claims del usuario con info de Google
            var existingClaims = await userManager.GetClaimsAsync(user);

            // Remover claims antiguos de Google si existen
            var googleClaims = existingClaims.Where(c =>
                c.Type == "google_id" ||
                c.Type == "google_picture" ||
                c.Type == "google_name").ToList();

            if (googleClaims.Any())
            {
                await userManager.RemoveClaimsAsync(user, googleClaims);
            }

            // Agregar nuevos claims
            var claims = new List<Claim>
        {
            new("google_id", userInfo.Id),
            new("google_picture", userInfo.Picture),
            new("google_name", userInfo.Name)
        };

            await userManager.AddClaimsAsync(user, claims);

            // 5. Obtener roles y generar token
            var roles = await userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles, userInfo.Name, userInfo.Picture);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles
            };

            return (response, true);
        }
        catch (Exception ex)
        {
            // Log the exception if you have logging configured
            Console.WriteLine($"Error en GoogleCallbackAsync: {ex.Message}");
            return (null, false);
        }
    }

    private async Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code)
    {
        var client = httpClientFactory.CreateClient();

        var requestData = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")! },
            { "client_secret", Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")! },
            { "redirect_uri", Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI")! },
            { "grant_type", "authorization_code" }
        };

        var response = await client.PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(requestData)
        );

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al intercambiar code por tokens: {errorContent}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
        return tokenResponse ?? throw new Exception("No se pudo obtener el token de Google");
    }

    private async Task<GoogleUserInfoDto> GetGoogleUserInfoAsync(string accessToken)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al obtener info del usuario: {errorContent}");
        }

        var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfoDto>();
        return userInfo ?? throw new Exception("No se pudo obtener la información del usuario");
    }




    public async Task SaveRefreshTokenAsync(string userId, string refreshToken, int expiresIn)
    {
        var existingToken = await context.UserGoogleTokens.FirstOrDefaultAsync(t => t.UserId == userId);

        if (existingToken != null)
        {
            // Actualizar token existente
            existingToken.RefreshToken = refreshToken;
            existingToken.ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
        }
        else
        {
            // Crear nuevo token
            var newToken = new UserGoogleToken
            {
                UserId = userId,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn),
                CreatedAt = DateTime.UtcNow
            };

            context.UserGoogleTokens.Add(newToken);
        }

        await context.SaveChangesAsync();
    }


    private static string GenerateJwtToken(IdentityUser user, IList<string> roles, string? displayName = null, string? picture = null)
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

        // Agregar nombre y foto de Google si existen
        if (!string.IsNullOrEmpty(displayName))
        {
            claims.Add(new Claim("display_name", displayName));
        }

        if (!string.IsNullOrEmpty(picture))
        {
            claims.Add(new Claim("picture", picture));
        }

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