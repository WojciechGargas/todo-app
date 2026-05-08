using Microsoft.Extensions.DependencyInjection;
using Todo.Core.DomainServices;

namespace Todo.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        
        return services;
    }
}
