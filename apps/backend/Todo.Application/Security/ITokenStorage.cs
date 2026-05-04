using Todo.Application.DTO;

namespace Todo.Application.Security;

public interface ITokenStorage
{
    void Set(JwtDto jwt);
    JwtDto? Get();
}