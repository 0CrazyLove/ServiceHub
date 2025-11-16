namespace Backend.DTOs;

public class GoogleAuthCodeDto
{
    public string Code { get; set; } = string.Empty;
}

public class GoogleUserInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public bool VerifiedEmail { get; set; }
}

public class GoogleTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }

}