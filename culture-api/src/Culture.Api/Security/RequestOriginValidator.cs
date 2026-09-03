namespace Culture.Api.Security;

public sealed class RequestOriginValidator(IConfiguration configuration)
{
    private readonly HashSet<string> _allowedOrigins = configuration.GetSection("Security:AllowedOrigins")
        .Get<string[]>()
        ?.Select(Normalize)
        .ToHashSet(StringComparer.OrdinalIgnoreCase)
        ?? [];

    public bool IsAllowed(HttpRequest request)
    {
        string? origin = request.Headers.Origin.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(origin))
        {
            return _allowedOrigins.Contains(Normalize(origin));
        }

        string? referer = request.Headers.Referer.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(referer) && Uri.TryCreate(referer, UriKind.Absolute, out Uri? uri))
        {
            return _allowedOrigins.Contains(Normalize(uri.GetLeftPart(UriPartial.Authority)));
        }

        return false;
    }

    private static string Normalize(string origin) => origin.Trim().TrimEnd('/');
}
