using Todo.Application.Abstractions;
using Todo.Application.DTO;

namespace Todo.Application.Quaries;

public class GetUser : IQuery<UserDto>
{
    public Guid UserId { get; set; }
}