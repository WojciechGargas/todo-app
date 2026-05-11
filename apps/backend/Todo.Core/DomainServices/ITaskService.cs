using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Core.DomainServices;

public interface ITaskService
{
    Task AddTaskAsync(User user, TaskId taskId, string name, string description);
    Task DeleteTaskAsync(User user, TodoTask task);
}
