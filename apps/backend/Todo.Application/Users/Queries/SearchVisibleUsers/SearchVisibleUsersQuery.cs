using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Core.ValueObjects;

namespace Todo.Application.Users.Queries.SearchVisibleUsers;

public sealed class SearchVisibleUsersQuery : IQuery<IReadOnlyList<UserShareCandidateDto>>
{
    public UserId RequestedUserId { get; init; }
    public string? Query { get; init; }
}
