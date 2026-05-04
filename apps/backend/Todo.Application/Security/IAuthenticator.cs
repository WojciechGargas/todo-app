using Todo.Application.DTO;

namespace Todo.Application.Security;

public interface IAuthenticator
{
    JwtDto CreateToken(Guid userId, string role);
}