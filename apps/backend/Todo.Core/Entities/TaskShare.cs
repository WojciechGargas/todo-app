using Todo.Core.Enums;
using Todo.Core.Exceptions;
using Todo.Core.ValueObjects;

namespace Todo.Core.Entities;

public class TaskShare
{
    public TaskId TaskId { get; private set; }
    public UserId UserId { get; private set; }
    public UserId SharedByUserId { get; private set; }
    public TaskSharePermission Permission { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public TaskShare(TaskId taskId, UserId userId, UserId sharedByUserId, TaskSharePermission permission,
        DateTime createdAt)
    {
        if (userId == sharedByUserId)
            throw new CannotShareTaskWithSelfException();
        
        TaskId = taskId;
        UserId = userId;
        SharedByUserId = sharedByUserId;
        Permission = permission;
        CreatedAt = createdAt;
    }
    
    public void ChangePermission(TaskSharePermission permission)
        => Permission = permission;
    
}