using Todo.Application.Abstractions;
using Todo.Application.DTO;

namespace Todo.Application.Users.Queries.GetUserWithTasks;

public class GetUserWithTasksQuery : IQuery<UserWithTasksDto>
{
    public Guid UserId { get; set; }
}
