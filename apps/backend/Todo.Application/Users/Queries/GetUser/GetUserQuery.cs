using Todo.Application.Abstractions;
using Todo.Application.DTO;

namespace Todo.Application.Users.Queries.GetUser;

public class GetUserQuery : IQuery<UserDto>
{
    public Guid UserId { get; set; }
}
