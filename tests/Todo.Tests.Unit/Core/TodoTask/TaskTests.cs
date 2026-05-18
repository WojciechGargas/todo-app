using Todo.Tests.Unit.Shared;

namespace Todo.Tests.Unit.Core.TodoTask;

public class TaskTests
{
    [Fact]
    public void MarkTaskAsDone_WhenTaskExists_SetsIsCompletedToTrue()
    {
        //Arrange
        var task = TaskTestData.CreateOwnedByOwner();
        
        //Act
        task.MarkAsCompleted();
        
        //Assert
        Assert.True(task.IsCompleted);
    }
    
    [Fact]
    public void MarkTaskAsDone_WhenTaskExistsAndIsAlreadyTrue_RemainsTrue()
    {
        //Arrange
        var task = TaskTestData.CreateOwnedByOwner();
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
        var task = TaskTestData.CreateOwnedByOwner();
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
        var task = TaskTestData.CreateOwnedByOwner();
        
        //Act
        task.MarkAsUncompleted();
        
        //Assert
        Assert.False(task.IsCompleted);
    }

    [Fact]
    public void ChangeTaskName_WhenTaskExists_UpdatesTaskName()
    {
        //Arrange
        var task = TaskTestData.CreateOwnedByOwner();
        var newTaskName = "updatedName";
        
        //Act
        task.ChangeName(newTaskName);
        
        //Assert
        Assert.Equal(newTaskName, task.TaskName);
    }
    
    [Fact]
    public void ChangeTaskDescription_WhenTaskExists_UpdatesTaskDescription()
    {
        //Arrange
        var task = TaskTestData.CreateOwnedByOwner();
        var newTaskDescription = "updatedDescription";
        
        //Act
        task.ChangeDescription(newTaskDescription);
        
        //Assert
        Assert.Equal(newTaskDescription, task.TaskDescription);
    }
}