namespace UrlShortener.Domain.Entities;

public sealed class ClickEvent
{
    private ClickEvent() { }
    public ClickEvent(long shortUrlId, DateTime clickedAtUtc, string? ipAddress, string? userAgent)
    {
        ShortUrlId = shortUrlId; ClickedAtUtc = clickedAtUtc; IpAddress = ipAddress; UserAgent = userAgent;
    }
    public long Id { get; private set; }
    public long ShortUrlId { get; private set; }
    public DateTime ClickedAtUtc { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
}
