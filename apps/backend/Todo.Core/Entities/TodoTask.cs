using Todo.Core.ValueObjects;

namespace Todo.Core.Entities;

public class TodoTask
{
    private readonly List<UserId> _sharedWithUserIds = new();
    
    public TaskId TaskId {get; private set;}
    public UserId OwnerUserId {get; private set;}
    public TaskName TaskName { get; private set; }
    public TaskDescription TaskDescription { get; private set; }
    public bool IsCompleted { get; private set; }
    public IReadOnlyCollection<UserId> SharedWithUserIds => _sharedWithUserIds;

    public TodoTask(TaskId taskId, UserId ownerUserId, TaskName taskName,
        TaskDescription taskDescription)
    {
        TaskId = taskId;
        OwnerUserId = ownerUserId;
        TaskName = taskName;
        TaskDescription = taskDescription;
    }
    
    public void ShareWith(UserId userId)
    {
        if (userId == OwnerUserId) return;
        if (!_sharedWithUserIds.Contains(userId)) return;
        _sharedWithUserIds.Add(userId);
    }

    public void UnshareWith(UserId userId)
    {
        if (!_sharedWithUserIds.Contains(userId)) return;
        _sharedWithUserIds.Remove(userId);
    }
    
    public void ChangeName(string newName)
        => TaskName = new TaskName(newName);
    
    public void ChangeDescription(string newDescription)
        => TaskDescription = new TaskDescription(newDescription);

    public void MarkAsCompleted()
    {
        if(IsCompleted)  return;
        IsCompleted = true;
    }
    
    public void MarkAsUncompleted()
    {
        if(!IsCompleted)  return;
        IsCompleted = false;
    }
}