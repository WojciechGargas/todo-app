using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Todo.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserIdOrThrow(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? throw new UnauthorizedAccessException("Missing user id claim.");

        if (!Guid.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user id claim.");

        return userId;
    }
    
    public static string GetTokenIdOrThrow(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(JwtRegisteredClaimNames.Jti)
               ?? throw new UnauthorizedAccessException("Missing token id claim.");
    }

    public static DateTime GetTokenExpirationUtcOrThrow(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(JwtRegisteredClaimNames.Exp)
                  ?? throw new UnauthorizedAccessException("Missing token expiration claim.");

        if (!long.TryParse(raw, out var unixSeconds))
        {
            throw new UnauthorizedAccessException("Invalid token expiration claim.");
        }

        return DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");
}