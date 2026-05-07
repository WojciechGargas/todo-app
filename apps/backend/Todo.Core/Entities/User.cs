using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Core.Entities;

public class User
{
    private readonly List<TaskId> _taskIds = new();

    public UserId UserId { get; private set; }
    public Email Email { get; private set; }
    public Username Username { get; private set; }
    public Password Password { get; private set; }
    public FullName FullName { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsEmailConfirmed { get; private set; }
    public DateTime? LastLoggedAtUtc { get; private set; }
    public IReadOnlyList<TaskId> TaskIds => _taskIds;

    public User(UserId userId, Email email, Username username, Password password,
        FullName fullName, UserRole role, DateTime createdAt, bool isEmailConfirmed = false)
    {
        UserId = userId;
        Email = email;
        Username = username;
        Password = password;
        FullName = fullName;
        Role = role;
        CreatedAt = createdAt;
        IsEmailConfirmed = isEmailConfirmed;
    }

    public void AddTask(TaskId taskId)
    {
        if (_taskIds.Contains(taskId))
        {
            return;
        }

        _taskIds.Add(taskId);
    }

    public void RemoveTask(TaskId taskId)
    {
        if (!_taskIds.Contains(taskId))
        {
            return;
        }

        _taskIds.Remove(taskId);
    }

    public void MarkEmailAsConfirmed()
        => IsEmailConfirmed = true;

    public void ChangeFullName(string newFullname)
        => FullName = new FullName(newFullname);

    public void ChangeEmail(string newEmail)
    {
        Email = new Email(newEmail);
        IsEmailConfirmed = false;
    }

    public void ChangePassword(Password newSecuredPassword)
        =>  Password = newSecuredPassword;
    
    public void ChangeUsername(string newUsername)
        => Username = new Username(newUsername);
}
