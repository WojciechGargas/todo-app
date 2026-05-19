using Todo.Core.Enums;
using Todo.Core.Exceptions;
using Todo.Core.ValueObjects;
using Todo.Tests.Unit.Shared;

namespace Todo.Tests.Unit.Core.TaskShare;

public class TaskShareTests
{
    [Fact]
    public void Ctor_WhenUserIdEqualsSharedByUserId_ThrowsCannotShareTaskWithSelfException()
    {
        //Arrange
        var sharedUserId = new UserId(IdsTestData.UserOwnerId);

        //Act
        var exception = Record.Exception(() => new Todo.Core.Entities.TaskShare(
            new TaskId(IdsTestData.TaskOwnerTodoId),
            sharedUserId,
            sharedUserId,
            TaskSharePermission.Read,
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

        //Assert
        Assert.IsType<CannotShareTaskWithSelfException>(exception);
    }

    [Theory]
    [InlineData(TaskSharePermission.Read, TaskSharePermission.Edit)]
    [InlineData(TaskSharePermission.Edit, TaskSharePermission.Read)]
    public void ChangePermission_WhenCalled_UpdatesPermission(
        TaskSharePermission initialPermission,
        TaskSharePermission targetPermission)
    {
        //Arrange
        var share = TaskShareTestData.CreateOwnerToCollaborator(permission: initialPermission);
        Assert.Equal(initialPermission, share.Permission);

        //Act
        share.ChangePermission(targetPermission);

        //Assert
        Assert.Equal(targetPermission, share.Permission);
    }
}