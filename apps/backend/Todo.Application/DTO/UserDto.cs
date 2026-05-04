using Todo.Core.Enums;

namespace Todo.Application.DTO;

public class UserDto
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string UserName { get; private set; }
    public UserRole Role { get; private set; }
}