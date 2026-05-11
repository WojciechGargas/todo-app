using Todo.Application.Abstractions;
using Todo.Application.DTO;

namespace Todo.Application.TodoTasks.Queries.GetTasks;

public class GetTasksQuery : IQuery<IReadOnlyList<TodoTaskDto>>
{
    public Guid UserId { get; set; }
}