using Todo.Core.Entities;
using Todo.Core.Enums;

namespace Todo.Core.Policies;

public class TaskSharePolicy : ITaskSharePolicy
{
    public bool CanShare(TodoTask task, User user)
    {
        if (user.Role == UserRole.Admin || task.OwnerUserId == user.UserId)
            return true;
        
        return false;
    }
}