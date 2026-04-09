using Todo.Core.ValueObjects;

namespace Todo.Core.Entities;

public class TodoTask
{
    private readonly List<UserId> _sharedWithUserIds = new();
    
    public TaskName TaskName { get; private set; }
    public TaskDescription TaskDescription { get; private set; }
    public bool IsCompleted { get; private set; }
    public IReadOnlyCollection<UserId> SharedWithUserIds => _sharedWithUserIds;
}