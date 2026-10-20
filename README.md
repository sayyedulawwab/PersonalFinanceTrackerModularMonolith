# PersonalFinanceTracker

A .NET 10 modular monolith for personal finance. It currently has two modules: **Users** (registration, login, JWT and refresh tokens, profile) and **Accounts** (CRUD for financial accounts with a money balance). Modules share only a small common kernel and do not reference each other.

## Tech stack

- .NET 10, ASP.NET Core Web API with URL/header API versioning
- EF Core 10 with PostgreSQL (Npgsql, snake_case naming)
- JWT bearer authentication
- Serilog, Seq and OpenTelemetry for logging and tracing
- FluentValidation, Scrutor, Swagger
- Docker Compose and a dev container

## Structure

```
src/
  API/                      host: controllers, middleware, Program.cs
  Common/                   shared kernel (Domain, Application, Infrastructure)
  Modules/
    Users/                  Modules.Users.{Domain,Application,Infrastructure}
    Accounts/               Modules.Accounts.{Domain,Application,Infrastructure}
tests/Modules/Users/UnitTests/   domain and application unit tests
```

Each module has its own `DbContext` (implementing `IUnitOfWork`) and migrations, but both use the same PostgreSQL database. Commands and queries are handled without MediatR; handlers are registered by assembly scan and injected into controller actions. Logging and validation are applied as decorators.

## Prerequisites

- .NET 10 SDK (`global.json` pins 10.0.100 with roll-forward)
- Docker, for PostgreSQL and Seq (or your own instances)

## Running locally

With everything in containers (PostgreSQL, Seq and the API):

```bash
docker compose up --build
```

Seq's UI is on host port 8081 and the API is published on host ports 5000 and 8080.

With the API on the host, start PostgreSQL (port 5432, database `financeDB`) and Seq (port 5341) first, for example `docker compose up database-postgres logging-seq`, then:

```bash
dotnet run --project src/API                        # http://localhost:5180, Swagger at /swagger
dotnet run --project src/API --launch-profile https # https://localhost:7088
```

Build and test:

```bash
dotnet build PersonalFinanceTracker.slnx
dotnet test PersonalFinanceTracker.slnx
```

Only the Users module has tests. The build treats warnings as errors and runs Sonar analyzers.

Migrations (one `DbContext` per module, the API project is the startup project):

```bash
dotnet ef migrations add <Name> --project src/Modules/Users/Modules.Users.Infrastructure --startup-project src/API --context UsersDbContext
dotnet ef migrations add <Name> --project src/Modules/Accounts/Modules.Accounts.Infrastructure --startup-project src/API --context AccountsDbContext
```

## Configuration

`src/API/appsettings.json` holds only environment-agnostic settings (Serilog levels, `AllowedHosts`). Everything environment-specific lives in `src/API/appsettings.Development.json`:

- `ConnectionStrings:Database`
- `Jwt:Secret`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiresInMinutes`
- `Seq:ServerUrl` and the Serilog Seq sink

Outside Development the app has no connection string or JWT secret and fails at startup. Provide them as environment variables (`ConnectionStrings__Database`, `Jwt__Secret`, `Seq__ServerUrl`, `Serilog__WriteTo__1__Args__serverUrl`, ...) or user secrets. `docker-compose.yml` sets these for the container network and they override the `localhost` values from the Development file.

## API overview

Routes are versioned: `api/v1/...`, or send an `X-Api-Version` header. Swagger is available in Development only.

| Method and route | Description |
|---|---|
| `POST /api/v1/auth/register` | Register a user |
| `POST /api/v1/auth/login` | Log in and receive tokens |
| `POST /api/v1/auth/refresh-token` | Exchange a refresh token for new tokens |
| `DELETE /api/v1/auth/refresh-tokens` | Revoke a user's refresh tokens |
| `GET /api/v1/users` | List users |
| `GET /api/v1/users/{userId}` | Get a user |
| `PUT /api/v1/users/{userId}` | Update a user |
| `GET /api/v1/accounts` | List accounts |
| `GET /api/v1/accounts/{accountId}` | Get an account |
| `POST /api/v1/accounts` | Create an account |
| `PUT /api/v1/accounts/{accountId}` | Update an account |
| `DELETE /api/v1/accounts/{accountId}` | Delete an account |

Currencies supported for account balances are `USD` and `BDT`.

## Known limitations

- **Development wipes the database on every start.** `ApplyMigrations()` calls `EnsureDeleted()` before migrating, and both modules share one database, so all users and accounts are lost on each Development run.
- No endpoint has `[Authorize]`. JWT bearer is registered, but nothing is protected, and the Accounts endpoints take `UserId` from the request body. `DELETE /auth/refresh-tokens` takes `userId` from the query string.
- Modules do not communicate: `Account.UserId` is a plain `Guid` with no foreign key and no check that the user exists. Domain-event plumbing exists but nothing dispatches events.
- No FluentValidation validators are defined yet.
- There is no transaction entity or endpoint.
- Trace export uses the default OTLP endpoint (`localhost:4317`, or `OTEL_EXPORTER_OTLP_ENDPOINT`); nothing in the compose file listens there.
