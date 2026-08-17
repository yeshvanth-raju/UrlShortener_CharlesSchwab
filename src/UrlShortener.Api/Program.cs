using System.Threading.RateLimiting;
using UrlShortener.Api.Middleware;
using UrlShortener.Application.Services;
using UrlShortener.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<UrlShortenerService>();
builder.Services.AddScoped<RedirectService>();
builder.Services.AddScoped<AnalyticsService>();

builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = 429;
    o.AddPolicy("fixed", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 120,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("fixed");
app.MapHealthChecks("/health");
app.Run();

public partial class Program { }
