# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

ASP.NET Core Web API on `net10.0` (`global.json` pins SDK 10.0.100, `rollForward: latestFeature`), solution `PersonalFinanceTracker.slnx`. The README is a title only.

```bash
dotnet build PersonalFinanceTracker.slnx
dotnet test PersonalFinanceTracker.slnx
dotnet run --project src/API                        # http://localhost:5180, Swagger at /swagger (Development only)
dotnet run --project src/API --launch-profile https # https://localhost:7088

dotnet test tests/Modules/Users/UnitTests/Modules.Users.Domain.UnitTests
dotnet test tests/Modules/Users/UnitTests/Modules.Users.Application.UnitTests
dotnet test <project> --filter "FullyQualifiedName~LoginUserQueryHandlerTests"

# EF Core migrations: one DbContext per module, API is the startup project (needs the dotnet-ef tool; no tool manifest)
dotnet ef migrations add <Name> --project src/Modules/Users/Modules.Users.Infrastructure --startup-project src/API --context UsersDbContext
dotnet ef migrations add <Name> --project src/Modules/Accounts/Modules.Accounts.Infrastructure --startup-project src/API --context AccountsDbContext

docker compose up --build   # Postgres + Seq + API
```

[Directory.Build.props](Directory.Build.props) sets `TreatWarningsAsErrors`, `AnalysisMode=All`, `EnforceCodeStyleInBuild`, and references `SonarAnalyzer.CSharp` at a pinned version. NuGet vulnerability advisories (NU190x) are errors too, so a new advisory can break restore without any code change; bump the package. [.editorconfig](.editorconfig) sets most style rules to `:error` and treats `**/Migrations/**` as generated code. EF Core and Npgsql are 10.0.x.

## Running it

Backing services: PostgreSQL (database `financeDB`) and Seq (ingest on 5341, UI on host port 8081), both defined in [docker-compose.yml](docker-compose.yml). [src/API/appsettings.json](src/API/appsettings.json) holds only environment-agnostic settings (Serilog console sink and levels, `AllowedHosts`). Everything environment-specific lives in [src/API/appsettings.Development.json](src/API/appsettings.Development.json): `ConnectionStrings:Database`, `Jwt:Secret`/`Issuer`/`Audience`/`ExpiresInMinutes`, `Seq:ServerUrl` and the Serilog Seq sink (`Serilog:WriteTo:1`). That file has development credentials committed and points at `localhost`. Outside Development the app has no connection string or JWT secret and fails at startup, so supply them as env vars (`ConnectionStrings__Database`, `Jwt__Secret`, `Seq__ServerUrl`, ...) or user secrets (the API project has a `UserSecretsId`).

- Compose and the dev container set the container-network values as env vars (`ConnectionStrings__Database` with host `database-postgres`, `Seq__ServerUrl` and `Serilog__WriteTo__1__Args__serverUrl` with host `logging-seq`, `Jwt__Secret`), which override the `localhost` values from the Development file. For `dotnet run` on the host, start Postgres on 5432 and Seq on 5341, or override the same variables.
- In Development, `ApplyMigrations()` ([MigrationExtensions.cs](src/API/Extensions/MigrationExtensions.cs)) runs at startup with up to 10 retries 5 s apart. It calls `EnsureDeleted()` on `UsersDbContext` first, and both modules share one database, so **every Development start wipes all data** (users and accounts) before migrating both contexts.
- Logs go to the console and to Seq twice: the `Serilog:WriteTo` Seq sink, and the OTLP log sink in [Program.cs](src/API/Program.cs) that posts to `{Seq:ServerUrl}/ingest/otlp/v1/logs`. Trace export uses `AddOtlpExporter()` with no endpoint (default `localhost:4317`, or `OTEL_EXPORTER_OTLP_ENDPOINT`), and nothing in compose listens there.
- Compose: the API is published on host ports 5000/5001 by `docker-compose.yml` (5001 maps the container's 8082, which nothing listens on) and also on 8080 by `docker-compose.override.yml`, which serves plain HTTP only (no certificate in the image, so do not set `ASPNETCORE_HTTPS_PORTS`). The override is set up for Visual Studio's compose tooling (`docker-compose.dcproj`). The root `launchSettings.json` and the dcproj name the service `api`, but the compose service is `finance-tracker-api`. The Seq image needs `SEQ_FIRSTRUN_NOAUTHENTICATION` (set in compose) or an admin password, or it exits at start.
- [.devcontainer/](.devcontainer/) has its own compose file (Postgres, Seq, and an SDK container that sleeps). Inside it, VS Code's launch config builds `src/API` and debugs with vsdbg.
- The Dockerfile copies every `.csproj` explicitly before restoring, so a new project must be added there too.

## Architecture

Modular monolith. There is one host (`src/API`) and shared kernel projects `src/Common/Common.{Domain,Application,Infrastructure}`. Each module is `src/Modules/<M>/Modules.<M>.{Domain,Application,Infrastructure}`. Modules: **Users** (registration, login, JWT and refresh tokens, profile) and **Accounts** (CRUD for financial accounts with a `Money` balance). Only Users has tests.

References run Application→Domain and Infrastructure→Application. API references each module's Application and Infrastructure. Modules reference only `Common.*`, never each other.

### Module wiring

- [Program.cs](src/API/Program.cs) finds every `IModuleInstaller` (in `Common.Infrastructure`) across loaded assemblies plus an explicit `moduleAssemblies` array, and calls `Install`. Each module's `<M>ModuleInstaller` then instantiates every `IServiceInstaller` (in `Common.Application`) found in its own Infrastructure and Application assemblies, via each project's `AssemblyReference`. The comments in `UsersModuleInstaller` saying "Common.*" are wrong: it scans the Users assemblies.
- To add a module, add its installer's assembly to `moduleAssemblies`, reference it from `API.csproj`, add it to `ApplyMigrations()`, and add its csproj files to the Dockerfile.
- Installer order is Users → Accounts → Common: `Program.cs` sorts `CommonModuleInstaller` last (printed to the console at startup). This matters because `CommonApplicationServiceInstaller` calls Scrutor `TryDecorate` for `LoggingDecorator` and `ValidationDecorator`, which only decorates handlers already registered. No FluentValidation validators exist yet, so `ValidationDecorator` is a no-op in practice.
- Each module registers its DbContext as a keyed `IUnitOfWork` (keys `"Users"` and `"Accounts"`), and its handlers take `[FromKeyedServices("<Module>")] IUnitOfWork`. There is deliberately no unkeyed `IUnitOfWork`. A new module must follow the same pattern, and unit tests can still substitute plain `IUnitOfWork`.

### How modules communicate

They don't. There are no integration events, public module APIs, or shared tables. `Account.UserId` is a plain `Guid` with no foreign key and no check that the user exists. Domain-event plumbing exists (`Entity.RaiseDomainEvent`, `IDomainEventHandler<T>`), but nothing raises or dispatches events: neither `SaveChangesAsync` override publishes them.

### Persistence

Each module has its own DbContext in its Infrastructure project. The DbContext implements `IUnitOfWork`, applies `IEntityTypeConfiguration` classes from `Configurations/`, and has migrations in `Migrations/`. Both DbContexts use the same `Database` connection string, the default `public` schema, and a shared `__EFMigrationsHistory` table. Npgsql with `UseSnakeCaseNamingConvention()`, so tables and columns are snake_case (`users`, `refresh_tokens`, `accounts`). `Money` is an owned type on `Account`, and `Currency` is stored as its code (only `USD` and `BDT` are valid for `Currency.FromCode`). The Users model snapshot class is named `ApplicationDbContextModelSnapshot`.

### Request flow and conventions

- CQRS without MediatR or a dispatcher. Commands implement `ICommand`/`ICommand<T>`, queries implement `IQuery<T>`, and handlers return `Result`/`Result<T>`; a `T` converts implicitly. Each module's `ApplicationServiceInstaller` registers handlers with a Scrutor scan, so new handlers are picked up automatically. Use-case folders: `Application/<Area>/<Verb>/` (e.g. `Accounts/Create/`).
- Controllers live in the API project under `src/API/Modules/<M>/<Area>/`, with request DTO records next to them. An action takes the handler as a parameter (e.g. `ICommandHandler<CreateAccountCommand, Guid> handler`), maps the request to a command, and on failure returns `result.Error.ToActionResult()` ([ErrorMappingExtensions.cs](src/API/Extensions/ErrorMappingExtensions.cs)).
- `Error(Name, Description, Code)` carries a custom `HttpResponseStatusCodes` enum (400/404/409/500). Define new errors in the entity's `*Errors` static class with `Error.NotFound/BadRequest/Conflict(...)`.
- Routes are versioned: `api/v{v:apiVersion}/...` with `[ApiVersion(1)]`, or the `X-Api-Version` header.
- `ExceptionHandlingMiddleware` turns a thrown FluentValidation `ValidationException` into a 400 `ProblemDetails`, and anything else into a 500. `RequestContextLoggingMiddleware` pushes `X-Correlation-Id` (or the trace id) into the Serilog log context.
- Entities derive from `Entity<Guid>`, have private constructors, and expose static `Create`/`Update(entity, ...)` methods that mutate and return the entity. Handlers take time from `IDateTimeProvider`.
- Login and refresh-token are implemented as **queries** (`LoginUserQuery`, `LoginUserWithRefreshTokenQuery`) even though they write refresh tokens.

## Known bugs and mismatches

- No endpoint has `[Authorize]`. JWT bearer is registered in Users Infrastructure, but nothing is protected, and the Accounts endpoints take `UserId` from the request body.
- `DELETE /auth/refresh-tokens` takes `userId` from the query string.
- `RevokeRefreshTokensCommandHandler` uses `ExecuteDeleteAsync` and returns the user id even when nothing was deleted.
- `ApplyMigrations()` wipes the database on every Development start (see Running it).
- There is no Transaction entity or endpoint; the main flows are register/login/refresh, users, and accounts.
