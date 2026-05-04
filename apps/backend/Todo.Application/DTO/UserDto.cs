using Todo.Core.Enums;

namespace Todo.Application.DTO;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public UserRole Role { get;  set; }
}