namespace Backend.Configurations;

public class GoogleSettings
{
    public GoogleSettings()
    {
        ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")!;
        ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")!;
        RedirectUri = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI")!;
    }
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;

}

