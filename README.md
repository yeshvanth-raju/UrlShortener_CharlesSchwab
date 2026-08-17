# URL Shortener

Production-minded modular monolith using ASP.NET Core/.NET 8, EF Core and SQLite.

## Structure

- `UrlShortener.Domain` - entities and domain rules
- `UrlShortener.Application` - use cases, contracts and abstractions
- `UrlShortener.Infrastructure` - EF Core, SQLite and technical implementations
- `UrlShortener.Api` - HTTP/API concerns
- `UrlShortener.UnitTests` - unit tests
- `UrlShortener.IntegrationTests` - API tests

## Run

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/UrlShortener.Api
```

## API

Create:

```http
POST /api/v1/urls
Content-Type: application/json

{
  "url": "https://example.com/path",
  "customAlias": "demo",
  "expiresAtUtc": null
}
```

Redirect:

```text
GET /demo
```

Analytics:

```text
GET /api/v1/urls/demo/analytics
```

Health:

```text
GET /health
```

## Engineering decisions

- HTTP/HTTPS only.
- Database unique index is the authoritative short-code uniqueness guarantee.
- Click count is incremented atomically.
- UTC timestamps.
- Bounded alias/URL/user-agent input.
- Problem Details error responses.
- IP-based fixed-window rate limiting for the prototype.
- Analytics failure does not fail a valid redirect.
- SQLite is intentionally used for the assignment; PostgreSQL/SQL Server is the natural production replacement.
- Redis and asynchronous analytics/outbox processing are documented as scale-out options rather than unnecessary MVP infrastructure.

AI was used as an implementation/review accelerator. Architecture, concurrency, validation, security and trade-offs were human-reviewed and intentionally kept pragmatic.
