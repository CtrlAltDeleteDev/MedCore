namespace MedCore.Auth.IdentityServer.Configuration;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    // Store in user-secrets (dev) or env var JwtSettings__SecretKey (prod). Never in appsettings.json.
    public string SecretKey { get; init; } = string.Empty;

    public int ExpirationMinutes { get; init; } = 60;
}
