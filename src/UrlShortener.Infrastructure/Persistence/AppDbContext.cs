using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();
    public DbSet<ClickEvent> ClickEvents => Set<ClickEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortUrl>(b =>
        {
            b.ToTable("ShortUrls");
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
            b.Property(x => x.OriginalUrl).HasMaxLength(2048).IsRequired();
        });

        modelBuilder.Entity<ClickEvent>(b =>
        {
            b.ToTable("ClickEvents");
            b.HasKey(x => x.Id);
            b.Property(x => x.IpAddress).HasMaxLength(64);
            b.Property(x => x.UserAgent).HasMaxLength(512);
            b.HasIndex(x => new { x.ShortUrlId, x.ClickedAtUtc });
            b.HasOne<ShortUrl>().WithMany().HasForeignKey(x => x.ShortUrlId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
