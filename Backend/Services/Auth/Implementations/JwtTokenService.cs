using Backend.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Services.Auth.Interfaces;

namespace Backend.Services.Auth.Implementations;

/// <summary>
/// Service responsible for JWT token generation and management.
/// </summary>
public class JwtTokenService(JwtSettings jwtSettings) : IJwtTokenService
{
    /// <summary>
    /// Generate JWT bearer token for authentication.
    /// </summary>
    public string GenerateToken(IdentityUser user, IList<string> roles, string? displayName = null, string? picture = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.UserName!)
        };

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
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}