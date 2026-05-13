using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Todo.Application.Security;

namespace Todo.Infrastructure.Auth;

internal static class Extensions
{
    private const string SectionName = "auth";

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetRequiredSection(SectionName));
        services.AddSingleton<ITokenStorage, HttpContextTokenStorage>();
        services.AddScoped<ITokenRevocationService, PostgresTokenRevocationService>();
        var options = configuration.GetOptions<AuthOptions>(SectionName);

        services
            .AddSingleton<IAuthenticator, Authenticator>()
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Audience = options.Audience;
                x.IncludeErrorDetails = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = options.Issuer,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey))
                };
                
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenId = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);

                        if (string.IsNullOrWhiteSpace(tokenId))
                        {
                            context.Fail("Missing token id.");
                            return;
                        }

                        var tokenRevocationService = context.HttpContext.RequestServices
                            .GetRequiredService<ITokenRevocationService>();

                        if (await tokenRevocationService.IsTokenRevokedAsync(tokenId))
                        {
                            context.Fail("Token has been revoked.");
                        }
                    }
                };
            });
        
        services.AddAuthorization(authorization =>
        {
            authorization.AddPolicy("RequireAdminRole", policy =>
            {
                policy.RequireRole("Admin");
            });
        });
        
        return services;
    }
}