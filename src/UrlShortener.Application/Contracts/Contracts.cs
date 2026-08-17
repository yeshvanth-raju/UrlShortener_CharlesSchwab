namespace UrlShortener.Application.Contracts;

public sealed record CreateShortUrlRequest(string? Url, string? CustomAlias, DateTime? ExpiresAtUtc);
public sealed record CreateShortUrlResponse(string Code, string OriginalUrl, DateTime CreatedAtUtc, DateTime? ExpiresAtUtc, string ShortUrl);
public sealed record DailyClickCount(DateOnly Date, long Count);
public sealed record AnalyticsResponse(string Code, long TotalClicks, IReadOnlyList<DailyClickCount> DailyClicks);
