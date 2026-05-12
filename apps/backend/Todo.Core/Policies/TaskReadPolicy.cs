using Todo.Core.Entities;
using Todo.Core.Enums;

namespace Todo.Core.Policies;

public class TaskReadPolicy : ITaskReadPolicy
{
    public bool CanRead(TodoTask task, User user, TaskShare? share)
    {
        if (user.Role == UserRole.Admin || task.OwnerUserId == user.UserId)
            return true;
        
        return share is not null;
    }
}