namespace Todo.Application.DTO;

public record TodoTaskDto(
    Guid Id,
    string Name,
    string Description,
    bool IsComplete);