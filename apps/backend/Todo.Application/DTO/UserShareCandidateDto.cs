namespace Todo.Application.DTO;

public sealed record UserShareCandidateDto(
    Guid Id,
    string UserName,
    string FullName);