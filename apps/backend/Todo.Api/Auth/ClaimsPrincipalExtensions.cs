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

    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");
}