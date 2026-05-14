# todo-app

Backend API for todo-app built with .NET, EF Core, PostgreSQL, JWT authentication, and Hangfire jobs.

## What This App Does

todo-app is a collaborative task manager. Users can create and manage their own todo tasks, update task details, and delete completed items. Tasks can also be shared with other users with specific permissions, so more than one person can work on the same items. The app supports signup/signin, email confirmation, JWT-based authentication, logout with token revocation, and automatic cleanup of expired revoked tokens in the background.

## Project Structure

- `apps/backend/Todo.Api` - API host, controllers, app startup
- `apps/backend/Todo.Application` - application layer (commands/queries, DTOs)
- `apps/backend/Todo.Core` - domain model and business rules
- `apps/backend/Todo.Infrastructure` - database, auth, integrations, background jobs

## Requirements

- .NET SDK 10
- PostgreSQL (local or Docker)

## Run Locally

From `apps/backend`:

```powershell
dotnet restore
dotnet ef database update --project .\Todo.Infrastructure\Todo.Infrastructure.csproj --startup-project .\Todo.Api\Todo.Api.csproj
dotnet run --project .\Todo.Api\Todo.Api.csproj
```

Swagger is available at:

- `https://localhost:<port>/swagger`

## Configuration

Main configuration is in:

- `apps/backend/Todo.Api/appsettings.json`
- `apps/backend/Todo.Api/appsettings.Development.json`
- optional local overrides: `appsettings.Development.local.json`

Important sections:

- `postgres` - PostgreSQL connection string
- `auth` - JWT settings
- `cors` - allowed frontend origins
- `hangfire` - background job configuration

Example `hangfire` section:

```json
"hangfire": {
  "schema": "hangfire",
  "cleanupCron": "0 12 * * *",
  "dashboardEnabled": true
}
```

## Migrations

Create migration:

```powershell
dotnet ef migrations add MigrationName --project .\Todo.Infrastructure\Todo.Infrastructure.csproj --startup-project .\Todo.Api\Todo.Api.csproj --output-dir DAL\Migrations
```

Apply migrations:

```powershell
dotnet ef database update --project .\Todo.Infrastructure\Todo.Infrastructure.csproj --startup-project .\Todo.Api\Todo.Api.csproj
```

## Hangfire

The backend uses Hangfire with PostgreSQL storage.

- Recurring job: cleanup of expired revoked tokens
- Schedule: controlled by `hangfire.cleanupCron`
- Dashboard endpoint: `/hangfire`
- Dashboard access: authenticated `Admin` role only
- Dashboard exposure: controlled by `hangfire.dashboardEnabled`
