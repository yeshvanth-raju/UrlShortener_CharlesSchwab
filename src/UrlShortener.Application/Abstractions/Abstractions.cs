using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Abstractions;

public interface IShortUrlRepository
{
    Task AddAsync(ShortUrl entity, CancellationToken cancellationToken);
    Task<ShortUrl?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken);
    Task IncrementClickCountAsync(long id, CancellationToken cancellationToken);
}

public interface IClickEventRepository
{
    Task AddAsync(ClickEvent clickEvent, CancellationToken cancellationToken);
    Task<IReadOnlyList<ClickEvent>> GetForUrlAsync(long shortUrlId, DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken);
}

public interface IShortCodeGenerator { string Generate(); }
public interface IClock { DateTime UtcNow { get; } }
