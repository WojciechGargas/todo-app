using Todo.Core.Exceptions;
using Todo.Tests.Unit.Shared;

namespace Todo.Tests.Unit.Core.TodoTask;

public class TodoTaskTests
{
    [Fact]
    public void MarkTaskAsDone_WhenTaskExists_SetsIsCompletedToTrue()
    {
        //Arrange
        var task = TodoTaskTestData.CreateOwnedByOwner();
        
        //Act
        task.MarkAsCompleted();
        
        //Assert
        Assert.True(task.IsCompleted);
    }
    
    [Fact]
    public void MarkTaskAsDone_WhenTaskExistsAndIsAlreadyTrue_RemainsTrue()
    {
        //Arrange
        var task = TodoTaskTestData.CreateOwnedByOwner();
        task.MarkAsCompleted();
        
        //Act
        task.MarkAsCompleted();
        
        //Assert
        Assert.True(task.IsCompleted);
    }
    
    [Fact]
    public void MarkTaskAsNotDone_WhenTaskExists_SetsIsCompletedToFalse()
    {
        //Arrange
        var task = TodoTaskTestData.CreateOwnedByOwner();
        task.MarkAsCompleted();
        Assert.True(task.IsCompleted);
        
        //Act
        task.MarkAsUncompleted();
        
        //Assert
        Assert.False(task.IsCompleted);
    }
    
    [Fact]
    public void MarkTaskAsNotDone_WhenTaskExistsAndIsAlreadyFalse_RemainsFalse()
    {
        //Arrange
        var task = TodoTaskTestData.CreateOwnedByOwner();
        
        //Act
        task.MarkAsUncompleted();
        
        //Assert
        Assert.False(task.IsCompleted);
    }

    [Fact]
    public void ChangeTaskName_WhenTaskExists_WithValidData_UpdatesTaskName()
    {
        //Arrange
        var task = TodoTaskTestData.CreateOwnedByOwner();
        const string newTaskName = "updatedName";
        
        //Act
        task.ChangeName(newTaskName);
        
        //Assert
        Assert.Equal(newTaskName, task.TaskName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijk")]
    public void ChangeTaskName_WhenTaskExists_WithInvalidData_ThrowsInvalidTodoTaskNameException
        (string invalidData)
    {
        //Arrange
        var task = TodoTaskTestData.CreateOwnedByOwner();
        
        //Act
        var exception = Record.Exception(() => task.ChangeName(invalidData));
        
        //Assert
        Assert.IsType<InvalidTodoTaskNameException>(exception);
    }
    
    [Fact]
    public void ChangeTaskDescription_WhenTaskExists_UpdatesTaskDescription()
    {
        //Arrange
        var task = TodoTaskTestData.CreateOwnedByOwner();
        const string newTaskDescription = "updatedDescription";
        
        //Act
        task.ChangeDescription(newTaskDescription);
        
        //Assert
        Assert.Equal(newTaskDescription, task.TaskDescription);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz")]
    public void ChangeTaskDescription_WhenTaskExists_WithInvalidData_ThrowsInvalidTaskDescriptionException
        (string invalidData)
    {
        //Arrange
        var task = TodoTaskTestData.CreateOwnedByOwner();
        
        //Act
        var exception = Record.Exception(() => task.ChangeDescription(invalidData));
        
        //Assert
        Assert.IsType<InvalidTaskDescriptionException>(exception);
    }
}