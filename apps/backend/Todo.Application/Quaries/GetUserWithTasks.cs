using Todo.Application.Abstractions;
using Todo.Application.DTO;

namespace Todo.Application.Quaries;

public class GetUserWithTasks : IQuery<UserWithTasksDto>
{
    public Guid UserId { get; set; }
}