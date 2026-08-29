# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A .NET 10 microservices e-commerce system: an AuthApi, ProductApi, and CartApi sitting behind a YARP-based ApiGateway, plus a server-rendered Blazor frontend (`E_Commerce_Web`) that talks only to the gateway. Auth and Product share a SQL Server database via EF Core; Cart is backed by Redis.

## Commands

Build/restore is standard `dotnet` CLI, run from the repo root (`E_Commerce.sln`).

```bash
dotnet restore
dotnet build E_Commerce.sln
dotnet run --project AuthApi        # http://localhost:5001
dotnet run --project ProductApi     # http://localhost:5002
dotnet run --project CartApi        # http://localhost:5222 / https 7073
dotnet run --project ApiGateway     # https://localhost:5000 (Swagger at /gateway)
dotnet run --project E_Commerce_Web # http://localhost:5159 (Blazor Server UI)
```

Tests (xUnit + Moq + FluentAssertions):

```bash
dotnet test                                   # all test projects
dotnet test Auth.Test/Auth.Test.csproj
dotnet test Product.Test/Product.Test.csproj
dotnet test --filter "FullyQualifiedName~ClassName.MethodName"   # single test
```

EF Core migrations (run from the project holding the DbContext's assembly, or with `--project`):

```bash
dotnet ef migrations add <Name> --project AuthApi.Data --startup-project AuthApi
dotnet ef database update --project AuthApi.Data --startup-project AuthApi

dotnet ef migrations add <Name> --project ProductApi.Infrastructure --startup-project ProductApi
dotnet ef database update --project ProductApi.Infrastructure --startup-project ProductApi
```
AuthApi and ProductApi both call `db.Database.Migrate()` at startup, so migrations also apply automatically on run.

Docker (SQL Server + ProductApi only — Auth/Cart/Gateway/Web are not wired into `docker-compose.yml` yet):

```bash
docker-compose up
```

## Architecture

**Per-service layering** — every business service (Auth, Product, Cart) is split into the same three layers as separate class libraries, named `<Service>.<Layer>`:
- `*.Models` — entities and `DTO/` classes only, no logic.
- `*.Infrastructure` / `*.Data` — `DbContext` (or `RedisConnection` for Cart), EF migrations, and `Repository/` + `IRepository/` implementations.
- The Web SDK project (`AuthApi`, `ProductApi`, `CartApi`) — `Controllers/` + `Program.cs` only; controllers depend on repository interfaces, never on `DbContext` directly.

Product and Auth additionally depend on the shared **`ECommerce.Data`** / **`ECommerce.Models`** projects:
- `ECommerce.Models.ApiResponse<T>` is the uniform response envelope (`Success`, `Message`, `Data`, `Error`, `StatusCode`) returned from essentially every controller action via `ApiResponse<T>.SuccessResponse(...)` / `.FailResponse(...)`.
- `ECommerce.Models.BaseEntity` gives every entity a `Guid Id`, `CreatedAt`, `UpdatedAt`.
- `ECommerce.Data.GenericRepository<TEntity> : IGenericRepository<TEntity>` implements generic CRUD (`GetAllAsync`/`FindSingleAsync` take predicate + `params` include expressions) over any `BaseEntity`; concrete repos like `ProductRepository`/`CategoryRepository` extend it for entity-specific queries.
- `ECommerce.Data.Middleware.GlobalExceptionMiddleware` (registered via `app.UseGlobalExceptionHandler()`) is the shared exception handler used by AuthApi and ProductApi.
- `ECommerce.Data.Profiles` holds shared AutoMapper profiles (`AuthProfile`, `ProductProfile`), wired up per-service with `AddCommonAutoMapper(typeof(<Profile>).Assembly)`.
- `CartApi` does **not** use `ECommerce.Data`/`ECommerce.Models` — it's an independent stack backed by `StackExchange.Redis` rather than EF Core, with its own repository/service pair (`ICartRepository`/`CartRepository`, `CartService.Application.Services.CartService`).

**ApiGateway** is a YARP reverse proxy (`ApiGateway/appsettings.Development.json` → `ReverseProxy` section) plus its own centralized JWT bearer authentication:
- Routes: `/api/product/**` and `/api/Categories/**` → `product-cluster` (ProductApi on :5002); `/api/auth/**` → `auth-cluster` (AuthApi on :5001). CartApi is not yet proxied.
- `/api/Categories` requires the `AdminOnly` policy (role `Admin`); the gateway also defines `SuperAdminOnly` and `UserOrAdmin` policies for use as routes grow.
- JWT `Key`/`Issuer`/`Audience` are configured identically in the gateway's `appsettings` — token validation happens centrally at the gateway, not (currently) re-validated in each downstream API.
- Swagger docs are aggregated: gateway routes `/swagger/product/**` and `/swagger/auth/**` (with `PathRemovePrefix`/`PathPrefix` transforms) through to each service's own `/swagger` endpoint, surfaced together at `/gateway`.
- Add a new proxied service by adding a route + cluster entry here, not by editing the downstream service.

**E_Commerce_Web** (Blazor Server, `.razor` under `Components/`) never calls a backend service directly — it uses a named `HttpClient("Gateway")` (base address `https://localhost:5000/api/`) with `JwtAuthorizationMessageHandler` attaching the bearer token from `TokenService`/`JwtAuthenticationStateProvider` (`Authentication/`) to every request. Pages are split under `Components/Pages/Public` (storefront), `Components/Pages/Account` (login/register), with `AdminLayout`/`AdminSidebar` reserved for an admin area.

**Solution file drift**: `E_Commerce.slnx` (the newer XML solution format) omits the `CartApi*` projects and `AuthService.Data`/`ProductApi.Application` (currently unused stub libraries) that are present in `E_Commerce.sln`. Use `E_Commerce.sln` if you need the full project set via CLI.

**`ECommerce.Caching`** is an in-progress Redis-caching library (meant to sit alongside `ECommerce.Data`/`ECommerce.Models` for Auth/Product) that is not yet wired up: it isn't referenced by `E_Commerce.sln`/`.slnx` or by any other project's `.csproj`, and its own types don't compile together yet (`CachingServiceExtensions.AddRedisCaching` registers `ICacheService`/`RedisCacheService`, but `ICacheService.cs` currently only defines an empty `Class1` and `RedisCacheService` is `internal` and implements nothing). Treat it as scaffolding, not a usable dependency, until it's finished and added to a project.
