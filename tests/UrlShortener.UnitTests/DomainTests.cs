using UrlShortener.Domain.Entities;

namespace UrlShortener.UnitTests;

public sealed class DomainTests
{
    [Fact]
    public void ExpiredUrl_CannotRedirect()
    {
        var now = DateTime.UtcNow;
        var url = ShortUrl.Create("abc", "https://example.com", now.AddHours(-2), now.AddHours(-1));
        Assert.False(url.CanRedirect(now));
    }

    [Fact]
    public void DisabledUrl_CannotRedirect()
    {
        var now = DateTime.UtcNow;
        var url = ShortUrl.Create("abc", "https://example.com", now, null);
        url.Disable();
        Assert.False(url.CanRedirect(now));
    }
}
