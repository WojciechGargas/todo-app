using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Core.Entities;

public class User
{
    public UserId UserId { get; private set; }
    public Email Email { get; private set; }
    public Username Username { get; private set; }
    public Password Password { get; private set; }
    public FullName FullName { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoggedAtUtc { get; private set; }
}