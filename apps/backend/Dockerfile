FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ToDoApp.slnx ./
COPY Todo.Api/Todo.Api.csproj Todo.Api/
COPY Todo.Application/Todo.Application.csproj Todo.Application/
COPY Todo.Core/Todo.Core.csproj Todo.Core/
COPY Todo.Infrastructure/Todo.Infrastructure.csproj Todo.Infrastructure/

RUN dotnet restore Todo.Api/Todo.Api.csproj

COPY Todo.Api/ Todo.Api/
COPY Todo.Application/ Todo.Application/
COPY Todo.Core/ Todo.Core/
COPY Todo.Infrastructure/ Todo.Infrastructure/

RUN dotnet publish Todo.Api/Todo.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "Todo.Api.dll"]
