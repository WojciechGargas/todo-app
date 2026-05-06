using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Abstractions;
using Todo.Core.Abstractions;
using Todo.Infrastructure.Auth;
using Todo.Infrastructure.DAL;
using Todo.Infrastructure.Email;
using Todo.Infrastructure.Exceptions;
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

        var infrastructureAssembly = typeof(AppOptions).Assembly;

        services.Configure<AppOptions>(section)
            .AddScoped<ExceptionMiddleware>()
            .AddSecurity()
            .AddAuth(configuration)
            .AddEmailConfirmation(configuration)
            .AddPostgres(configuration)
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
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("frontend");
        app.UseAuthentication();
        app.UseAuthorization();
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
