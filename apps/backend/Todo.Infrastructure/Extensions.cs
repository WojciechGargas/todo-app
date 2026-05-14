using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Abstractions;
using Todo.Core.Abstractions;
using Todo.Infrastructure.Auth;
using Todo.Infrastructure.DAL;
using Todo.Infrastructure.Email;
using Todo.Infrastructure.Exceptions;
using Todo.Infrastructure.Hangfire;
using Todo.Infrastructure.Security;
using Todo.Infrastructure.Time;

namespace Todo.Infrastructure;

public static class Extensions
{
    private const string SectionName = "app";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var allowedOrigins = configuration.GetSection("cors:allowedOrigins").Get<string[]>() ?? [];
        
        var postgresOptions = configuration.GetOptions<PostgresOptions>("postgres");
        
        var infrastructureAssembly = typeof(AppOptions).Assembly;

        services.Configure<HangfireOptions>(configuration.GetSection(HangfireOptions.SectionName));
        services.Configure<AppOptions>(section)
            .AddScoped<ExceptionMiddleware>()
            .AddSecurity()
            .AddAuth(configuration)
            .AddEmailConfirmation(configuration)
            .AddPostgres(configuration)
            .AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(storageOptions =>
                    storageOptions.UseNpgsqlConnection(postgresOptions.ConnectionString));
            })
            .AddHangfireServer()
            .AddSingleton<IClock, Clock>()
            .Scan(s => s.FromAssemblies(infrastructureAssembly)
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime())
            .AddHttpContextAccessor()
            .AddSwaggerGen()
            .AddEndpointsApiExplorer();

        services.AddCors(options =>
        {
            options.AddPolicy("frontend", policy =>
            {
                if (allowedOrigins.Length == 0)
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    return;
                }

                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        var hangfireOptions = app.Services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<HangfireOptions>>()
            .Value;
        var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
        
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("frontend");
        app.UseAuthentication();
        app.UseAuthorization();
        recurringJobManager.AddOrUpdate<RevokedTokensCleanupJob>(
            "cleanup-revoked-tokens",
            job => job.ExecuteAsync(),
            hangfireOptions.CleanupCron);
        if (hangfireOptions.DashboardEnabled)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = [new HangfireDashboardAuthorizationFilter()]
            });
        }
        app.MapControllers();

        return app;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(options);

        return options;
    }
}
