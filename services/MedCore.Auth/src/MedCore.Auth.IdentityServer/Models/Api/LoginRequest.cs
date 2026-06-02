using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedCore.Auth.IdentityServer.Models.Api;

public sealed class LoginRequest
{
    [Required] [DefaultValue("patient1")]public string Username { get; init; } = string.Empty;
    [Required][DefaultValue("Pass123$")] public string Password { get; init; } = string.Empty;
}
