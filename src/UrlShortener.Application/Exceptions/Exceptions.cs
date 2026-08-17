namespace UrlShortener.Application.Exceptions;

public sealed class ValidationException(string message) : Exception(message);
public sealed class ShortUrlNotFoundException(string code) : Exception($"Short URL '{code}' was not found.");
public sealed class ShortUrlUnavailableException(string code) : Exception($"Short URL '{code}' is expired or disabled.");
public sealed class ShortCodeConflictException(string code) : Exception($"Short code '{code}' is already in use.");
public sealed class ShortCodeGenerationException : Exception
{
    public ShortCodeGenerationException() : base("Unable to generate a unique short code.") { }
}
