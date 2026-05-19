using Todo.Core.Enums;
using Todo.Tests.Unit.Shared;

namespace Todo.Tests.Unit.Core.TaskShares;

public class TaskShareCtorTests
{
    [Fact]
    public void Ctor_WhenDataIsValid_SetsAllProperties()
    {
        //Arrange
        var share = TaskShareTestData.CreateOwnerToCollaborator(permission: TaskSharePermission.Edit);

        //Act
        // ctor executed in CreateOwnerToCollaborator();

        //Assert
        Assert.Multiple(
            () => Assert.Equal(IdsTestData.TaskOwnerTodoId, share.TaskId.Value),
            () => Assert.Equal(IdsTestData.UserCollaboratorId, (Guid)share.UserId),
            () => Assert.Equal(IdsTestData.UserOwnerId, (Guid)share.SharedByUserId),
            () => Assert.Equal(TaskSharePermission.Edit, share.Permission),
            () => Assert.Equal(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), share.CreatedAt)
        );
    }
}
