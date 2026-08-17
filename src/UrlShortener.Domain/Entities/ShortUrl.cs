namespace UrlShortener.Domain.Entities;

public sealed class ShortUrl
{
    private ShortUrl() { }

    private ShortUrl(string code, string originalUrl, DateTime createdAtUtc, DateTime? expiresAtUtc)
    {
        Code = code; OriginalUrl = originalUrl; CreatedAtUtc = createdAtUtc; ExpiresAtUtc = expiresAtUtc;
    }

    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string OriginalUrl { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public bool IsDisabled { get; private set; }
    public long ClickCount { get; private set; }

    public static ShortUrl Create(string code, string originalUrl, DateTime nowUtc, DateTime? expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(originalUrl)) throw new ArgumentException("URL is required.", nameof(originalUrl));
        if (expiresAtUtc.HasValue && expiresAtUtc.Value <= nowUtc) throw new ArgumentException("Expiration must be in the future.", nameof(expiresAtUtc));
        return new ShortUrl(code, originalUrl, nowUtc, expiresAtUtc);
    }

    public bool CanRedirect(DateTime nowUtc) =>
        !IsDisabled && (!ExpiresAtUtc.HasValue || ExpiresAtUtc.Value > nowUtc);

    public void Disable() => IsDisabled = true;
}
