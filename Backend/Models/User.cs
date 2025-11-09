namespace Backend.Models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Token { get; set; }
    public string? Email { get; set; }
    public IList<string> Roles { get; set; } = []; // "Admin" or "Customer"
}
