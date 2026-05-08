using Todo.Application.Abstractions;
using Todo.Application.DTO;

namespace Todo.Application.Users.Queries.GetUsers;

public class GetUsersQuery : IQuery<IEnumerable<UserDto>>
{
}
