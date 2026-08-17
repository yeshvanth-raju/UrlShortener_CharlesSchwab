using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Exceptions;

namespace UrlShortener.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", context.TraceIdentifier);
            var (status, title) = ex switch
            {
                ValidationException => (400, "Invalid request"),
                ShortUrlNotFoundException => (404, "Short URL not found"),
                ShortUrlUnavailableException => (410, "Short URL unavailable"),
                ShortCodeConflictException => (409, "Short code already exists"),
                ShortCodeGenerationException => (503, "Unable to generate short code"),
                _ => (500, "An unexpected error occurred")
            };
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            var problem = new ProblemDetails { Status = status, Title = title, Detail = status == 500 ? "The server could not complete the request." : ex.Message, Instance = context.Request.Path };
            problem.Extensions["traceId"] = context.TraceIdentifier;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
