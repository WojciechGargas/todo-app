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
}