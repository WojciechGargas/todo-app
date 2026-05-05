# todo-app

#Migrations

dotnet ef migrations add MigrationName --project .\Todo.Infrastructure\Todo.Infrastructure.csproj --startup-project .\Todo.Api\Todo.Api.csproj --output-dir DAL\Migrations

dotnet ef database update --project .\Todo.Infrastructure\Todo.Infrastructure.csproj --startup-project .\Todo.Api\Todo.Api.csproj