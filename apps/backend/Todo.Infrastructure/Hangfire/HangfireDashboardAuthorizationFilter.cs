using Hangfire.Dashboard;

namespace Todo.Infrastructure.Hangfire;

internal sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var  httpContext = context.GetHttpContext();
        
        return httpContext.User.Identity?.IsAuthenticated == true
            && httpContext.User.IsInRole("Admin");
    }
}