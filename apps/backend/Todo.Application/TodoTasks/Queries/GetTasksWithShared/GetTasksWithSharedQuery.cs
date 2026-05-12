using Todo.Application.Abstractions;
using Todo.Application.DTO;

namespace Todo.Application.TodoTasks.Queries.GetTasksWithShared;

public class GetTasksWithSharedQuery : IQuery<IReadOnlyList<TodoTaskDto>>
{
    public Guid UserId { get; set; }
}