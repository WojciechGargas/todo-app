using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Core.Repositories;

public interface ITaskShareRepository
{
    Task<TaskShare?> GetShareAsync(TaskId taskId, UserId userId);
    Task<IReadOnlyList<TaskShare>> GetSharesForTaskAsync(TaskId taskId);
    Task<IReadOnlyList<TaskShare>> GetSharesForUserAsync(UserId userId);
    
    Task AddShareAsync(TaskShare share);
    Task DeleteShareAsync(TaskShare share);
    Task UpdatePermissionAsync(TaskId taskId, UserId userId, TaskSharePermission permission);
}