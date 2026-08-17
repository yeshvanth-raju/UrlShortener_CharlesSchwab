using UrlShortener.Application.Exceptions;
using UrlShortener.Application.Validation;

namespace UrlShortener.UnitTests;

public sealed class ValidationTests
{
    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com/a")]
    public void ValidUrlsPass(string url) => UrlValidator.Validate(url);

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("ftp://example.com")]
    [InlineData("not-a-url")]
    public void InvalidUrlsFail(string url) =>
        Assert.Throws<ValidationException>(() => UrlValidator.Validate(url));

    [Theory]
    [InlineData("abc123")]
    [InlineData("my-link")]
    [InlineData("my_link")]
    public void ValidAliasesPass(string alias) =>
        Assert.Equal(alias, AliasValidator.Validate(alias));

    [Fact]
    public void AliasWithSpacesFails() =>
        Assert.Throws<ValidationException>(() => AliasValidator.Validate("bad alias"));
}
