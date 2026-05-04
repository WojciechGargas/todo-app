using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Core.Entities;

public class User
{
    private readonly List<TaskId> _taskIds = new ();
    
    public UserId UserId { get; private set; }
    public Email Email { get; private set; }
    public Username Username { get; private set; }
    public Password Password { get; private set; }
    public FullName FullName { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoggedAtUtc { get; private set; }
    public IReadOnlyList<TaskId> TaskIds => _taskIds;

    public void AddTask(TaskId taskId)
    {
        if (_taskIds.Contains(taskId)) return;
        _taskIds.Add(taskId);
    }

    public void RemoveTask(TaskId taskId)
    {
        if (!_taskIds.Contains(taskId)) return;
        _taskIds.Remove(taskId);
    }
}