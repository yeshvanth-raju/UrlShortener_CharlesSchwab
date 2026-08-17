using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Exceptions;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Infrastructure.Repositories;

public sealed class ShortUrlRepository(AppDbContext db) : IShortUrlRepository
{
    public async Task AddAsync(ShortUrl entity, CancellationToken ct)
    {
        db.ShortUrls.Add(entity);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true)
        { throw new ShortCodeConflictException(entity.Code); }
    }

    public Task<ShortUrl?> GetByCodeAsync(string code, CancellationToken ct) =>
        db.ShortUrls.AsNoTracking().SingleOrDefaultAsync(x => x.Code == code, ct);

    public Task<bool> ExistsAsync(string code, CancellationToken ct) =>
        db.ShortUrls.AnyAsync(x => x.Code == code, ct);

    public Task IncrementClickCountAsync(long id, CancellationToken ct) =>
        db.ShortUrls.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.ClickCount, x => x.ClickCount + 1), ct);
}

public sealed class ClickEventRepository(AppDbContext db) : IClickEventRepository
{
    public async Task AddAsync(ClickEvent e, CancellationToken ct)
    {
        db.ClickEvents.Add(e);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ClickEvent>> GetForUrlAsync(long id, DateTime? from, DateTime? to, CancellationToken ct)
    {
        var q = db.ClickEvents.AsNoTracking().Where(x => x.ShortUrlId == id);
        if (from.HasValue) q = q.Where(x => x.ClickedAtUtc >= from.Value);
        if (to.HasValue) q = q.Where(x => x.ClickedAtUtc <= to.Value);
        return await q.OrderBy(x => x.ClickedAtUtc).ToListAsync(ct);
    }
}
