namespace Backend.Models;

public class UserGoogleToken
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}