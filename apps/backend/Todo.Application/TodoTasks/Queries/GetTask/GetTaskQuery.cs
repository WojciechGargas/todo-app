using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Queries.GetTask;

public class GetTaskQuery : IQuery<TodoTaskDto>
{
    public Guid UserId { get; set; }
    public TaskId Id { get; set; }
}
