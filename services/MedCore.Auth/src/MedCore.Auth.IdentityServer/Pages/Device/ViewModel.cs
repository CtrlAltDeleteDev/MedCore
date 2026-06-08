namespace MedCore.Auth.IdentityServer.Pages.Device;

public class ViewModel
{
    public string? ClientName { get; set; }

    public string? ClientUrl { get; set; }

    public string? ClientLogoUrl { get; set; }

    public bool AllowRememberConsent { get; set; }

    public IEnumerable<ScopeViewModel> IdentityScopes { get; set; } = Enumerable.Empty<ScopeViewModel>();

    public IEnumerable<ScopeViewModel> ApiScopes { get; set; } = Enumerable.Empty<ScopeViewModel>();
}

 #pragma warning disable SA1402
public class ScopeViewModel
 #pragma warning restore SA1402
{
    public string? Value { get; set; }

    public string? DisplayName { get; set; }

    public string? Description { get; set; }

    public bool Emphasize { get; set; }

    public bool Required { get; set; }

    public bool Checked { get; set; }
}
