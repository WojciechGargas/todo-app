using Todo.Application.Abstractions;
using Todo.Application.DTO;

namespace Todo.Application.Quaries;

public class GetUsers : IQuery<IEnumerable<UserDto>>
{
}