using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Tests.Unit.Shared;

public static class TaskShareTestData
{
    private static readonly DateTime DefaultCreatedAtUtc =
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static TaskShare CreateOwnerToCollaborator(
        Guid? taskId = null,
        Guid? userId = null,
        Guid? sharedByUserId = null,
        TaskSharePermission permission = TaskSharePermission.Read,
        DateTime? createdAtUtc = null)
        => new(
            new TaskId(taskId ?? IdsTestData.TaskOwnerTodoId),
            new UserId(userId ?? IdsTestData.UserCollaboratorId),
            new UserId(sharedByUserId ?? IdsTestData.UserOwnerId),
            permission,
            createdAtUtc ?? DefaultCreatedAtUtc);
}
