using Todo.Tests.Unit.Shared;

namespace Todo.Tests.Unit.Core.TodoTask;

public class TodoTaskCtorTests
{
    [Fact]
    public void Ctor_WhenDataIsValid_SetsAllProperties()
    {
        //Arrange
        var task = TodoTaskTestData.CreateOwnedByOwner();
        
        //Act
            //ctor executed in CreateOwnedByOwner();
            
        //Assert
        Assert.Multiple(
            () => Assert.Equal(IdsTestData.TaskOwnerTodoId, task.TaskId.Value),
            () => Assert.Equal(IdsTestData.UserOwnerId, (Guid)task.OwnerUserId),
            () => Assert.Equal("Owner task", (string)task.TaskName),
            () => Assert.Equal("Owner task description", (string)task.TaskDescription),
            () => Assert.False(task.IsCompleted)
        );
    }
}