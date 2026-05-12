using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Core.DomainServices;

public interface ITaskService
{
    Task AddTaskAsync(User user, TaskId taskId, string name, string description);
    Task DeleteTaskAsync(User user, TodoTask task);
    Task UpdateTaskAsync(User user, TodoTask task, string? name, string? description, bool? isComplete);
    Task ShareTaskAsync(User requestedBy, TodoTask task, UserId targetUserId, TaskSharePermission permission);
    Task UnshareTaskAsync(User user, TodoTask task, UserId targetUserId);
}
