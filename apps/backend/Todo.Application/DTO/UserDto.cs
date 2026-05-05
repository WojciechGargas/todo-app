using Todo.Core.Enums;

namespace Todo.Application.DTO;

public sealed record UserDto(
    Guid Id,
    string Email,
    string UserName,
    UserRole Role);
