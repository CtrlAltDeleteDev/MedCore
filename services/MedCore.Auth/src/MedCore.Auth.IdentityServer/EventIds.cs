namespace MedCore.Auth.IdentityServer.Pages;

internal static class EventIds
{
    //////////////////////////////
    // Consent
    //////////////////////////////
    public const int InvalidId = ConsentEventsStart + 0;
    public const int NoConsentMatchingRequest = ConsentEventsStart + 1;

    //////////////////////////////
    // External Login
    //////////////////////////////
    public const int ExternalClaims = ExternalLoginEventsStart + 0;

    //////////////////////////////
    // CIBA
    //////////////////////////////
    public const int InvalidBackchannelLoginId = CibaEventsStart + 0;
    public const int NoMatchingBackchannelLoginRequest = CibaEventsStart + 1;

    private const int UIEventsStart = 10000;
    private const int CibaEventsStart = UIEventsStart + 3000;
    private const int ExternalLoginEventsStart = UIEventsStart + 2000;
    private const int ConsentEventsStart = UIEventsStart + 1000;
}
