namespace Backend.DTOs;

public class UserGoogleTokenDto
{
    public string UserId { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class CreateUserGoogleTokenDto
{
    public string UserId { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}

public class UpdateUserGoogleTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}