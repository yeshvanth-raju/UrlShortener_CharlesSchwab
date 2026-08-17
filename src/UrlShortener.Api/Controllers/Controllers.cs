using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Contracts;
using UrlShortener.Application.Services;

namespace UrlShortener.Api.Controllers;

[ApiController, Route("api/v1/urls")]
public sealed class UrlsController(UrlShortenerService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateShortUrlResponse>> Create(CreateShortUrlRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, $"{Request.Scheme}://{Request.Host}", ct);
        return Created($"/{result.Code}", result);
    }
}

[ApiController]
public sealed class RedirectController(RedirectService service) : ControllerBase
{
    [HttpGet("/{code}")]
    public async Task<IActionResult> Redirect(string code, CancellationToken ct)
    {
        var url = await service.ResolveAsync(code, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString(), ct);
        return Redirect(url.OriginalUrl);
    }
}

[ApiController, Route("api/v1/urls/{code}/analytics")]
public sealed class AnalyticsController(AnalyticsService service) : ControllerBase
{
    [HttpGet]
    public Task<AnalyticsResponse> Get(string code, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct) =>
        service.GetAsync(code, fromUtc, toUtc, ct);
}
