using System.Net;
using System.Net.Http.Json;
using UrlShortener.Application.Contracts;

namespace UrlShortener.IntegrationTests;

public sealed class ApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    public ApiTests(ApiFactory factory) => _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task CreateUrl_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/urls", new CreateShortUrlRequest("https://example.com", null, null));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CreateShortUrlResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Code));
    }

    [Fact]
    public async Task InvalidScheme_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/urls", new CreateShortUrlRequest("javascript:alert(1)", null, null));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
