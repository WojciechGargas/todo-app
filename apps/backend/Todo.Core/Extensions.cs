using Microsoft.Extensions.DependencyInjection;
using Todo.Core.DomainServices;
using Todo.Core.Policies;

namespace Todo.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddSingleton<ITaskDeletionPolicy, TaskDeletionPolicy>();
        
        return services;
    }
}
