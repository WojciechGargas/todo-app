namespace Todo.Application.TodoTasks.Commands.UpdateTask;

public record UpdateTaskRequest(string? Name, string? Description, bool? IsCompleted);
