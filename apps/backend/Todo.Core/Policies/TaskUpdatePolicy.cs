using Todo.Core.Entities;
using Todo.Core.Enums;

namespace Todo.Core.Policies;

public class TaskUpdatePolicy : ITaskUpdatePolicy
{
    public bool CanUpdate(TodoTask task, User user)
        => user.Role == UserRole.Admin || task.OwnerUserId == user.UserId;
}