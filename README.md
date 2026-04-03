# FullStackHero .NET 10 Starter Kit

[![NuGet](https://img.shields.io/nuget/v/FullStackHero.CLI?label=fsh%20cli)](https://www.nuget.org/packages/FullStackHero.CLI)
[![NuGet](https://img.shields.io/nuget/v/FullStackHero.Framework.Web?label=framework)](https://www.nuget.org/packages/FullStackHero.Framework.Web)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

An opinionated, production-first starter for building multi-tenant SaaS and enterprise APIs on .NET 10. You get ready-to-ship Identity, Multitenancy, Auditing, caching, mailing, jobs, storage, health, OpenAPI, and OpenTelemetry—wired through Minimal APIs, Mediator, and EF Core.

## Quick Start

You get the complete source code — BuildingBlocks, Modules, and Playground — with full project references. No black-box NuGet packages; you own and can modify everything.

### Option 1: dotnet new template (recommended)

```bash
dotnet new install FullStackHero.NET.StarterKit
dotnet new fsh -n MyApp
cd MyApp
dotnet restore src/FSH.Framework.slnx
dotnet run --project src/Playground/FSH.Playground.AppHost
```

### Option 2: Clone the repository

```bash
git clone https://github.com/fullstackhero/dotnet-starter-kit.git MyApp
cd MyApp
dotnet restore src/FSH.Framework.slnx
dotnet run --project src/Playground/FSH.Playground.AppHost
```

> Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download), [Docker](https://www.docker.com/) (for Postgres/Redis via Aspire)

## Why teams pick this
- Modular vertical slices: drop `Modules.Identity`, `Modules.Multitenancy`, `Modules.Auditing` into any API and let the module loader wire endpoints.
- Battle-tested building blocks: persistence + specifications, distributed caching, mailing, jobs via Hangfire, storage abstractions, and web host primitives (auth, rate limiting, versioning, CORS, exception handling).
- Cloud-ready out of the box: Aspire AppHost spins up Postgres + Redis + the Playground API/Blazor with OTLP tracing enabled.
- Multi-tenant from day one: Finbuckle-powered tenancy across Identity and your module DbContexts; helpers to migrate and seed tenant databases on startup.
- Observability baked in: OpenTelemetry traces/metrics/logs, structured logging, health checks, and security/exception auditing.

## Stack highlights
- .NET 10, C# latest, Minimal APIs, Mediator for commands/queries, FluentValidation.
- EF Core 10 with domain events + specifications; Postgres by default, SQL Server ready.
- ASP.NET Identity with JWT issuance/refresh, roles/permissions, rate-limited auth endpoints.
- Hangfire for background jobs; Redis-backed distributed cache; pluggable storage.
- API versioning, rate limiting, CORS, security headers, OpenAPI (Swagger) + Scalar docs.

## Repository map
- `src/BuildingBlocks` — Core abstractions (DDD primitives, exceptions), Persistence, Caching, Mailing, Jobs, Storage, Web host wiring.
- `src/Modules` — `Identity`, `Multitenancy`, `Auditing` runtime + contracts projects.
- `src/Playground` — Reference host (`FSH.Api`), Aspire app host (`FSH.Playground.AppHost`), Blazor UI, Postgres migrations.
- `src/Tests` — Architecture tests that enforce layering and module boundaries.
- `docs/framework` — Deep dives on architecture, modules, and developer recipes.
- `terraform` — Infra as code scaffolding (optional starting point).

## Run it now (Aspire)
Prereqs: .NET 10 SDK, Aspire workload, Docker running (for Postgres/Redis).

1. Restore: `dotnet restore src/FSH.Framework.slnx`
2. Start everything: `dotnet run --project src/Playground/FSH.Playground.AppHost`
   - Aspire brings up Postgres + Redis containers, wires env vars, launches the Playground API and Blazor front end, and enables OTLP export on https://localhost:4317.
3. Hit the API: `https://localhost:5285` (Swagger/Scalar and module endpoints under `/api/v1/...`).

### Run the API only
- Set env vars or appsettings for `DatabaseOptions__Provider`, `DatabaseOptions__ConnectionString`, `DatabaseOptions__MigrationsAssembly`, `CachingOptions__Redis`, and JWT options.
- Run: `dotnet run --project src/Playground/FSH.Api`
- The host applies migrations/seeding via `UseHeroMultiTenantDatabases()` and maps module endpoints via `UseHeroPlatform`.

## Bring the framework into your API
- Reference the building block and module projects you need.
- In `Program.cs`:
  - Register Mediator with assemblies containing your commands/queries and module handlers.
  - Call `builder.AddHeroPlatform(...)` to enable auth, OpenAPI, caching, mailing, jobs, health, OTel, rate limiting.
  - Call `builder.AddModules(moduleAssemblies)` and `app.UseHeroPlatform(p => p.MapModules = true);`.
- Configure connection strings, Redis, JWT, CORS, and OTel endpoints via configuration. Example wiring lives in `src/Playground/FSH.Api/Program.cs`.

## Included modules
- **Identity** — ASP.NET Identity + JWT issuance/refresh, user/role/permission management, profile image storage, login/refresh auditing, health checks.
- **Multitenancy** — Tenant provisioning, migrations, status/upgrade APIs, tenant-aware EF Core contexts, health checks.
- **Auditing** — Security/exception/activity auditing with queryable endpoints; plugs into global exception handling and Identity events.

## Development notes
- Target framework: `net10.0`; nullable enabled; analyzers on.
- Tests: `dotnet test src/FSH.Framework.slnx` (includes architecture guardrails).
- Want the deeper story? Start with `docs/framework/architecture.md` and the developer cookbook in `docs/framework/developer-cookbook.md`.

Built and maintained by Mukesh Murugan for teams that want to ship faster without sacrificing architecture discipline.
