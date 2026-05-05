using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Application.DTO;

public sealed record UserWithTasksDto(Guid Id, string Email,
    string UserName, UserRole Role, IReadOnlyList<TodoTaskDto> Tasks);
