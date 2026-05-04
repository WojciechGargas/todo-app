using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Todo.Application.DTO;
using Todo.Application.Security;
using Todo.Core.Abstractions;

namespace Todo.Infrastructure.Auth;

public class Authenticator(IOptions<AuthOptions> options, IClock clock) : IAuthenticator
{
    private readonly string _issuer = options.Value.Issuer;
    private readonly string _audience = options.Value.Audience;
    private readonly TimeSpan _expiration = options.Value.Expiry ??  TimeSpan.FromHours(1);
    private readonly SigningCredentials _signingCredentials = new(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SigningKey)),
        SecurityAlgorithms.HmacSha256);
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtDto CreateToken(Guid userId, string role)
    {
        var now = clock.CurrentTimeUtc();
        var expires = now.Add(_expiration);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new(ClaimTypes.Role, role)
        };

        var jwt = new JwtSecurityToken(_issuer, _audience, claims, now, expires,
            _signingCredentials);
        var accessToken = _tokenHandler.WriteToken(jwt);
        
        return new JwtDto
        {
            AccessToken = accessToken,
        };
    }
}