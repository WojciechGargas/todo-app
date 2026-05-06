using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Email;

namespace Todo.Infrastructure.Email;

internal static class Extensions
{
    private const string ConfirmationSectionName = "emailConfirmation";
    private const string SmtpSectionName = "smtp";

    public static IServiceCollection AddEmailConfirmation(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailConfirmationOptions>(configuration.GetRequiredSection(ConfirmationSectionName));
        services.Configure<SmtpOptions>(configuration.GetRequiredSection(SmtpSectionName));
        services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();

        return services;
    }
}
