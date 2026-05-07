using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Core.Repositories;

public interface ITaskRepository
{
    Task<TodoTask?> GetTaskByIdAsync(TaskId taskId);
    Task AddTaskAsync(TodoTask task);
}