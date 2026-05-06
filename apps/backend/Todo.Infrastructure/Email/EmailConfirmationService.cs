using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Todo.Application.Email;
using Todo.Application.Exceptions;
using Todo.Core.Abstractions;

namespace Todo.Infrastructure.Email;

internal sealed class EmailConfirmationService(
    IOptions<EmailConfirmationOptions> confirmationOptionsAccessor,
    IOptions<SmtpOptions> smtpOptionsAccessor,
    IClock clock) : IEmailConfirmationService
{
    private readonly EmailConfirmationOptions _confirmationOptions = confirmationOptionsAccessor.Value;
    private readonly SmtpOptions _smtpOptions = smtpOptionsAccessor.Value;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public async Task SendRegistrationConfirmationAsync(Guid userId, string email)
    {
        var token = GenerateToken(userId, email, EmailConfirmationPurpose.Registration);
        var confirmationLink = BuildConfirmationLink(token);
        var body = $"Please confirm your registration by clicking this link: {confirmationLink}";
        await SendEmailAsync(email, "Confirm your account registration", body);
    }

    public async Task SendEmailChangeConfirmationAsync(Guid userId, string newEmail)
    {
        var token = GenerateToken(userId, newEmail, EmailConfirmationPurpose.ChangeEmail);
        var confirmationLink = BuildConfirmationLink(token);
        var body = $"Please confirm your email address change by clicking this link: {confirmationLink}";
        await SendEmailAsync(newEmail, "Confirm your email address change", body);
    }

    public EmailConfirmationPayload ReadToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidEmailConfirmationTokenException();
        }

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
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out _);

            var userIdRaw = FindClaimValue(principal, JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier);
            var email = FindClaimValue(principal, JwtRegisteredClaimNames.Email, ClaimTypes.Email);
            var purposeRaw = principal.FindFirstValue("purpose");

            if (!Guid.TryParse(userIdRaw, out var userId) ||
                string.IsNullOrWhiteSpace(email) ||
                !Enum.TryParse<EmailConfirmationPurpose>(purposeRaw, ignoreCase: true, out var purpose))
            {
                throw new InvalidEmailConfirmationTokenException();
            }

            return new EmailConfirmationPayload(userId, email, purpose);
        }
        catch (SecurityTokenException)
        {
            throw new InvalidEmailConfirmationTokenException();
        }
        catch (ArgumentException)
        {
            throw new InvalidEmailConfirmationTokenException();
        }
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

    private string GenerateToken(Guid userId, string email, EmailConfirmationPurpose purpose)
    {
        var now = clock.CurrentTimeUtc();
        var expires = now.Add(_confirmationOptions.Expiry ?? TimeSpan.FromHours(24));
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_confirmationOptions.SigningKey)),
            SecurityAlgorithms.HmacSha256);

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
            signingCredentials);

        return _tokenHandler.WriteToken(jwt);
    }

    private string BuildConfirmationLink(string token)
    {
        if (string.IsNullOrWhiteSpace(_confirmationOptions.FrontendConfirmUrl))
        {
            throw new InvalidOperationException("Email confirmation frontend URL is not configured.");
        }

        var separator = _confirmationOptions.FrontendConfirmUrl.Contains('?') ? "&" : "?";
        var encodedToken = WebUtility.UrlEncode(token);
        return $"{_confirmationOptions.FrontendConfirmUrl}{separator}token={encodedToken}";
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        using var message = new MailMessage
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
            From = new MailAddress(_smtpOptions.FromEmail, _smtpOptions.FromName)
        };

        message.To.Add(to);

        using var client = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port)
        {
            EnableSsl = _smtpOptions.UseSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_smtpOptions.Username, _smtpOptions.Password)
        };

        await client.SendMailAsync(message);
    }
}
