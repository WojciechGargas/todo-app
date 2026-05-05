using Todo.Core.ValueObjects;

namespace Todo.Application.DTO;

public record TodoTaskDto(
    TaskId Id,
    string Name,
    string Description,
    bool IsComplete);