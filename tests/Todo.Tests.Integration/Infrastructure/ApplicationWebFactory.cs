using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Todo.Application.Security;
using Todo.Core.Abstractions;
using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;
using Todo.Infrastructure.DAL;

namespace Todo.Tests.Integration.Infrastructure;

public sealed class ApplicationWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:17")
        .WithDatabase("todo_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    
    private string? _connectionString;
    
    private TestClock Clock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IClock>();
            services.AddSingleton<IClock>(Clock);
        });

        builder.ConfigureTestServices(testServices =>
        {
            var descriptor = testServices.SingleOrDefault(
                s => s.ServiceType == typeof(DbContextOptions<TodoDbContext>));

            if (descriptor is not null)
                testServices.Remove(descriptor);

            testServices.AddDbContext<TodoDbContext>(options =>
            {
                options.UseNpgsql(_connectionString ??
                                  throw new InvalidOperationException("Test database was not initialized"));
            });
        });
    }
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _connectionString = _dbContainer.GetConnectionString();
        
        await ResetDatabaseAsync();
    }
    
    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
    
    public Task ResetStateAsync() => ResetDatabaseAsync();
    
    private async Task ResetDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseNpgsql(_connectionString ?? throw new InvalidOperationException("Test database was not initialized"))
            .Options;
        
        await using var dbContext = new TodoDbContext(options);
        await dbContext.Database.MigrateAsync();

        await dbContext.TaskShares.ExecuteDeleteAsync();
        await dbContext.TodoTasks.ExecuteDeleteAsync();
        await dbContext.RevokedTokens.ExecuteDeleteAsync();
        await dbContext.Users.ExecuteDeleteAsync();
        
        await using var scope = Services.CreateAsyncScope();
        var passwordManager = scope.ServiceProvider.GetRequiredService<IPasswordManager>();
        
        var users = new List<User>
        {
            new(
                new UserId(Guid.Parse("11111111-1111-1111-1111-111111111111")), // owner
                new Email("owner@test.com"),
                new Username("owner_user"),
                new Password(passwordManager.Secure("Secret123!")),
                new FullName("Owner User"),
                UserRole.User,
                new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                isEmailConfirmed: true),

            new(
                new UserId(Guid.Parse("22222222-2222-2222-2222-222222222222")), // collaborator
                new Email("collab@test.com"),
                new Username("collab_user"),
                new Password(passwordManager.Secure("Secret123!")),
                new FullName("Collaborator User"),
                UserRole.User,
                new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                isEmailConfirmed: true),

            new(
                new UserId(Guid.Parse("33333333-3333-3333-3333-333333333333")), // admin
                new Email("admin@test.com"),
                new Username("admin_user"),
                new Password(passwordManager.Secure("Secret123!")),
                new FullName("Admin User"),
                UserRole.Admin,
                new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                isEmailConfirmed: true),
        };
        
        await dbContext.Users.AddRangeAsync(users);
        await dbContext.SaveChangesAsync();
    }
}