using Todo.Core.Entities;
using Todo.Core.Enums;

namespace Todo.Core.Policies;

public class TaskChangePermissionPolicy : ITaskChangePermissionPolicy
{
    public bool CanChange(TodoTask task, User user)
        => user.Role == UserRole.Admin || task.OwnerUserId == user.UserId;
}