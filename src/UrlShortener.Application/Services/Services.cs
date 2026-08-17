using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Contracts;
using UrlShortener.Application.Exceptions;
using UrlShortener.Application.Validation;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Services;

public sealed class UrlShortenerService(IShortUrlRepository repository, IShortCodeGenerator generator, IClock clock)
{
    public async Task<CreateShortUrlResponse> CreateAsync(CreateShortUrlRequest request, string baseUrl, CancellationToken ct)
    {
        UrlValidator.Validate(request.Url);
        var alias = AliasValidator.Validate(request.CustomAlias);
        var now = clock.UtcNow;

        if (request.ExpiresAtUtc.HasValue && request.ExpiresAtUtc.Value <= now)
            throw new ValidationException("Expiration must be in the future.");

        var code = alias ?? await GenerateCodeAsync(ct);
        if (await repository.ExistsAsync(code, ct)) throw new ShortCodeConflictException(code);

        var entity = ShortUrl.Create(code, request.Url!.Trim(), now, request.ExpiresAtUtc);
        await repository.AddAsync(entity, ct);

        return new CreateShortUrlResponse(entity.Code, entity.OriginalUrl, entity.CreatedAtUtc, entity.ExpiresAtUtc, $"{baseUrl.TrimEnd('/')}/{entity.Code}");
    }

    private async Task<string> GenerateCodeAsync(CancellationToken ct)
    {
        for (var i = 0; i < 10; i++)
        {
            var code = generator.Generate();
            if (!await repository.ExistsAsync(code, ct)) return code;
        }
        throw new ShortCodeGenerationException();
    }
}

public sealed class RedirectService(IShortUrlRepository urls, IClickEventRepository clicks, IClock clock)
{
    public async Task<ShortUrl> ResolveAsync(string code, string? ip, string? userAgent, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length > 32) throw new ShortUrlNotFoundException(code);
        var entity = await urls.GetByCodeAsync(code, ct) ?? throw new ShortUrlNotFoundException(code);
        if (!entity.CanRedirect(clock.UtcNow)) throw new ShortUrlUnavailableException(code);

        await urls.IncrementClickCountAsync(entity.Id, ct);

        try
        {
            await clicks.AddAsync(
                new ClickEvent(entity.Id, clock.UtcNow, Truncate(ip, 64), Truncate(userAgent, 512)), ct);
        }
        catch
        {
            // Analytics is secondary to the redirect.
        }

        return entity;
    }

    private static string? Truncate(string? value, int max) =>
        string.IsNullOrEmpty(value) || value.Length <= max ? value : value[..max];
}

public sealed class AnalyticsService(IShortUrlRepository urls, IClickEventRepository clicks)
{
    public async Task<AnalyticsResponse> GetAsync(string code, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
    {
        var url = await urls.GetByCodeAsync(code, ct) ?? throw new ShortUrlNotFoundException(code);
        if (fromUtc.HasValue && toUtc.HasValue && fromUtc > toUtc)
            throw new ValidationException("'fromUtc' cannot be after 'toUtc'.");

        var events = await clicks.GetForUrlAsync(url.Id, fromUtc, toUtc, ct);
        var daily = events.GroupBy(x => DateOnly.FromDateTime(x.ClickedAtUtc))
            .OrderBy(x => x.Key)
            .Select(x => new DailyClickCount(x.Key, x.LongCount()))
            .ToList();

        return new AnalyticsResponse(url.Code, events.Count, daily);
    }
}
