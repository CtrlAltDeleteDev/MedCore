namespace MedCore.Auth.IdentityServer.Models.Api;

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }
}
