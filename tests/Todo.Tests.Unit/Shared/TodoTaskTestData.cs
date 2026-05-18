using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Tests.Unit.Shared;

public static class TodoTaskTestData
{
    public static TodoTask CreateOwnedByOwner(
        Guid? taskId = null,
        Guid? ownerUserId = null,
        string? name = null,
        string? description = null)
        => new(
            new TaskId(taskId ?? IdsTestData.TaskOwnerTodoId),
            new UserId(ownerUserId ?? IdsTestData.UserOwnerId),
            new TaskName(name ?? "Owner task"),
            new TaskDescription(description ?? "Owner task description"));

    public static TodoTask CreateOwnedByCollaborator(
        Guid? taskId = null,
        Guid? ownerUserId = null,
        string? name = null,
        string? description = null)
        => new(
            new TaskId(taskId ?? IdsTestData.TaskSharedTodoId),
            new UserId(ownerUserId ?? IdsTestData.UserCollaboratorId),
            new TaskName(name ?? "Collaborator task"),
            new TaskDescription(description ?? "Collaborator task description"));
    
}
