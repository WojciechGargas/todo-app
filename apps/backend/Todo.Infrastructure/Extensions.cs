using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Abstractions;
using Todo.Core.Abstractions;
using Todo.Infrastructure.Auth;
using Todo.Infrastructure.DAL;
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

        var infrastructureAssembly = typeof(AppOptions).Assembly;

        services.Configure<AppOptions>(section)
            .AddScoped<ExceptionMiddleware>()
            .AddSecurity()
            .AddAuth(configuration)
            .AddPostgres(configuration)
            .AddSingleton<IClock, Clock>()
            .Scan(s => s.FromAssemblies(infrastructureAssembly)
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime())
            .AddHttpContextAccessor()
            .AddSwaggerGen()
            .AddEndpointsApiExplorer();
        
        return services;
    }
    
    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();
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