# API Labs

Nine small ASP.NET Core Web APIs, each built to practise one topic in isolation. Every
project is self-contained: its `.csproj` sits at the folder root, so `cd` into any of them
and run `dotnet run`.

Built with **.NET 10**.

## The projects

| Folder | Topic | URL |
|---|---|---|
| [auth-demo/](auth-demo/) | Basic authentication and JWT issuance, with a custom `AuthenticationHandler` | `https://localhost:7248` |
| [company/](company/) | EF Core with AutoMapper — departments and employees, DTO mapping, migrations | `https://localhost:7080` |
| [dapper-nhibernate/](dapper-nhibernate/) | Dapper and NHibernate side by side over the same schema | `https://localhost:7290` |
| [elastic-logging/](elastic-logging/) | Structured logging with Serilog shipped to Elasticsearch | `https://localhost:7164` |
| [email-rate-limit/](email-rate-limit/) | ASP.NET Core rate limiting on an email endpoint | `https://localhost:7131` |
| [expense-tracker/](expense-tracker/) | EF Core + SQL Server, containerised with Docker | `https://localhost:7044` |
| [hello-docker/](hello-docker/) | Minimal containerisation example — the smallest useful `Dockerfile` | `https://localhost:7085` |
| [minimal-api/](minimal-api/) | Minimal API routing without controllers | `https://localhost:7131` |
| [product/](product/) | DTOs over a service layer, with Swagger documentation | `https://localhost:7183` |

## Running one

```bash
cd company
dotnet run
```

Swagger UI is served at the root URL in development. Each project also ships a `.http`
file you can send requests from directly in Visual Studio or VS Code.

## Known issue

`email-rate-limit` and `minimal-api` are both configured on port `7131`, so they cannot run
at the same time. Change one in its `Properties/launchSettings.json` if you need both.
