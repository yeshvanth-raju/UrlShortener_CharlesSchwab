using System.Text.RegularExpressions;
using UrlShortener.Application.Exceptions;

namespace UrlShortener.Application.Validation;

public static class UrlValidator
{
    public static void Validate(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ValidationException("URL is required.");
        if (url.Length > 2048) throw new ValidationException("URL cannot exceed 2048 characters.");
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri)) throw new ValidationException("URL must be an absolute URL.");
        if (uri.Scheme is not (Uri.UriSchemeHttp or Uri.UriSchemeHttps))
            throw new ValidationException("Only HTTP and HTTPS URLs are supported.");
    }
}

public static partial class AliasValidator
{
    [GeneratedRegex("^[A-Za-z0-9_-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    public static string? Validate(string? alias)
    {
        if (string.IsNullOrWhiteSpace(alias)) return null;
        alias = alias.Trim();
        if (alias.Length > 32) throw new ValidationException("Custom alias cannot exceed 32 characters.");
        if (!Pattern().IsMatch(alias)) throw new ValidationException("Custom alias may contain only letters, digits, '-' and '_'.");
        return alias;
    }
}
