using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;
using Todo.Infrastructure.DAL;

namespace Todo.Tests.Integration.Controllers;

public partial class TasksControllerTests
{
    private static readonly Guid OwnerUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid AdminUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private async Task<(Guid OwnerTaskId, Guid OtherTaskId, Guid SharedTaskId)> SeedMixedTasksAsync(
        TaskSharePermission permission = TaskSharePermission.Read)
    {
        var ownerTaskId = Guid.NewGuid();
        var otherTaskId = Guid.NewGuid();
        var sharedTaskId = Guid.NewGuid();

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var now = factory.Clock.CurrentTimeUtc();

        db.TodoTasks.Add(new TodoTask(new TaskId(ownerTaskId), new UserId(OwnerUserId), new TaskName("Owner task"), new TaskDescription("Owned by owner")));
        db.TodoTasks.Add(new TodoTask(new TaskId(otherTaskId), new UserId(OtherUserId), new TaskName("Other task"), new TaskDescription("Owned by other")));
        db.TodoTasks.Add(new TodoTask(new TaskId(sharedTaskId), new UserId(OtherUserId), new TaskName("Shared task"), new TaskDescription("Shared to owner")));

        db.TaskShares.Add(new TaskShare(
            new TaskId(sharedTaskId),
            new UserId(OwnerUserId),
            new UserId(OtherUserId),
            permission,
            now));

        await db.SaveChangesAsync();
        return (ownerTaskId, otherTaskId, sharedTaskId);
    }

    private async Task<Guid[]> SeedOnlyOwnerTasksAsync(int count = 2)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        var ids = new List<Guid>(count);
        for (var i = 0; i < count; i++)
        {
            var id = Guid.NewGuid();
            ids.Add(id);
            db.TodoTasks.Add(new TodoTask(
                new TaskId(id),
                new UserId(OwnerUserId),
                new TaskName($"Owner task {i + 1}"),
                new TaskDescription("Owned by owner")));
        }

        await db.SaveChangesAsync();
        return ids.ToArray();
    }

    private async Task<Guid> SeedTaskForUserAsync(Guid ownerId, string name = "Task", string description = "Desc")
    {
        var taskId = Guid.NewGuid();

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        db.TodoTasks.Add(new TodoTask(
            new TaskId(taskId),
            new UserId(ownerId),
            new TaskName(name),
            new TaskDescription(description)));

        await db.SaveChangesAsync();
        return taskId;
    }

    private async Task SeedShareAsync(Guid taskId, Guid ownerId, Guid targetUserId, TaskSharePermission permission)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var now = factory.Clock.CurrentTimeUtc();

        db.TaskShares.Add(new TaskShare(
            new TaskId(taskId),
            new UserId(targetUserId),
            new UserId(ownerId),
            permission,
            now));

        await db.SaveChangesAsync();
    }

    private async Task<(Guid OwnerTaskId, Guid TargetTaskId)> SeedTwoUsersTasksNoShareAsync()
    {
        var ownerTaskId = await SeedTaskForUserAsync(OwnerUserId, "Owner private", "Owner only");
        var targetTaskId = await SeedTaskForUserAsync(OtherUserId, "Other private", "Other only");
        return (ownerTaskId, targetTaskId);
    }

    private async Task<(Guid SharedReadTaskId, Guid SharedEditTaskId)> SeedSharedReadAndEditTasksAsync()
    {
        var readTaskId = await SeedTaskForUserAsync(OtherUserId, "Shared read", "Read permission");
        var editTaskId = await SeedTaskForUserAsync(OtherUserId, "Shared edit", "Edit permission");

        await SeedShareAsync(readTaskId, OtherUserId, OwnerUserId, TaskSharePermission.Read);
        await SeedShareAsync(editTaskId, OtherUserId, OwnerUserId, TaskSharePermission.Edit);

        return (readTaskId, editTaskId);
    }

    private async Task<(Guid TaskId, Guid OwnerId, Guid TargetId)> SeedShareCandidateAsync(
        TaskSharePermission permission = TaskSharePermission.Read)
    {
        var taskId = await SeedTaskForUserAsync(OwnerUserId, "Share candidate", "To be shared");
        await SeedShareAsync(taskId, OwnerUserId, OtherUserId, permission);
        return (taskId, OwnerUserId, OtherUserId);
    }
}
