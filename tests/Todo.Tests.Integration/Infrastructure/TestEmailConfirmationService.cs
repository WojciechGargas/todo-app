using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Todo.Application.Email;
using Todo.Application.Exceptions;
using Todo.Core.Abstractions;
using Todo.Infrastructure.Email;

namespace Todo.Tests.Integration.Infrastructure;

public sealed class TestEmailConfirmationService(
    IOptions<EmailConfirmationOptions> confirmationOptionsAccessor,
    IClock clock)
    : IEmailConfirmationService

{
    private readonly EmailConfirmationOptions _confirmationOptions = confirmationOptionsAccessor.Value;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    
    public List<(Guid UserId, string Email, string Token)> RegistrationEmails { get; } = [];
    
    public Task SendRegistrationConfirmationAsync(Guid userId, string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var token = GenerateToken(userId, normalizedEmail, EmailConfirmationPurpose.Registration);
        RegistrationEmails.Add((userId, email, token));
        
        return Task.CompletedTask;
    }

    public Task SendEmailChangeConfirmationAsync(Guid userId, string newEmail)
    {
        var token = GenerateToken(userId, newEmail, EmailConfirmationPurpose.ChangeEmail);
        
        return Task.CompletedTask;
    }

    public EmailConfirmationPayload ReadToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidEmailConfirmationTokenException();
        
        var key = Encoding.UTF8.GetBytes(_confirmationOptions.SigningKey);

        try
        {
            var principal = _tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _confirmationOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _confirmationOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                LifetimeValidator = (_, expires, _, _) => expires is not null && expires > clock.CurrentTimeUtc(),
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out _);
            
            var userIdRaw = FindClaimValue(principal, JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier);
            var email = FindClaimValue(principal, JwtRegisteredClaimNames.Email, ClaimTypes.Email);
            var purposeRaw = principal.FindFirstValue("purpose");

            if (!Guid.TryParse(userIdRaw, out var userId) ||
                string.IsNullOrWhiteSpace(email) ||
                !Enum.TryParse<EmailConfirmationPurpose>(purposeRaw, true, out var purpose))
            {
                throw new InvalidEmailConfirmationTokenException();
            }
            
            return new EmailConfirmationPayload(userId, email, purpose);
        }
        catch
        {
            throw new InvalidEmailConfirmationTokenException();
        }
    }
    
    private string GenerateToken(Guid userId, string email, EmailConfirmationPurpose purpose)
    {
        var now = clock.CurrentTimeUtc();
        var expires = now.Add(_confirmationOptions.Expiry ?? TimeSpan.FromHours(24));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_confirmationOptions.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("purpose", purpose.ToString())
        };

        var jwt = new JwtSecurityToken(
            _confirmationOptions.Issuer,
            _confirmationOptions.Audience,
            claims,
            now,
            expires,
            creds);

        return _tokenHandler.WriteToken(jwt);
    }
    
    private static string? FindClaimValue(ClaimsPrincipal principal, params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var value = principal.FindFirstValue(claimType);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}