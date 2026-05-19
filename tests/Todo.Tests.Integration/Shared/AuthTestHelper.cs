using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Todo.Tests.Integration.Shared;

internal static class AuthTestHelper
{
    public static void ClearAuthorization(HttpClient client)
        => client.DefaultRequestHeaders.Authorization = null;

    public static void AuthenticateByJwt(HttpClient client, IServiceProvider services, Guid userId,
        string role = "User")
    {
        var authSettings = GetAuthSettings(services);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // API czyta sub
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")) // wymagane przez token revocation check
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: authSettings.Issuer,
            audience: authSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);
            
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);
    }
    
    private static AuthSettings GetAuthSettings(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var issuer = configuration["auth:issuer"]
                     ?? throw new InvalidOperationException("Missing configuration key: auth:issuer");
        var audience = configuration["auth:audience"]
                       ?? throw new InvalidOperationException("Missing configuration key: auth:audience");
        var signingKey = configuration["auth:signingKey"]
                         ?? throw new InvalidOperationException("Missing configuration key: auth:signingKey");

        return new AuthSettings(issuer, audience, signingKey);
    }

    private sealed record AuthSettings(string Issuer, string Audience, string SigningKey);
}