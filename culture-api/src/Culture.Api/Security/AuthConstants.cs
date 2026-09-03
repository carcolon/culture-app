namespace Culture.Api.Security;

public static class AuthConstants
{
    public const string SessionScheme = "Culture.Session";

    public const string AdminEntraScheme = "Culture.AdminEntra";

    public const string CsrfHeaderName = "X-CSRF-TOKEN";

    public const string CsrfCookieName = "__Host-culture-csrf";

    public const string SessionCookieName = "__Host-culture-session";
}
