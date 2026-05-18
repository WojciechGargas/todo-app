using Todo.Core.Exceptions;
using Todo.Core.ValueObjects;
using Todo.Tests.Unit.Shared;

namespace Todo.Tests.Unit.Core.User;

public class UserTests
{
    [Fact]
    public void AddTask_WhenTaskIsValid_AddsTask()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        var task = TodoTaskTestData.CreateOwnedByOwner();
        
        //Act
        user.AddTask(task.TaskId);
        
        //Assert
        Assert.Equal(task.TaskId, user.TaskIds[0]);
        Assert.Single(user.TaskIds);
    }

    [Fact]
    public void AddTask_WhenTaskAlreadyExists_DoesNotDuplicate()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        var task = TodoTaskTestData.CreateOwnedByOwner();
        user.AddTask(task.TaskId);
        
        //Act
        user.AddTask(task.TaskId);
        
        //Assert
        Assert.Equal(task.TaskId, user.TaskIds[0]);
        Assert.Single(user.TaskIds);
    }
    
    [Fact]
    public void RemoveTask_WhenTaskExists_RemovesTask()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        var task = TodoTaskTestData.CreateOwnedByOwner();
        user.AddTask(task.TaskId);
        
        //Act
        user.RemoveTask(task.TaskId);
        
        //Assert
        Assert.Empty(user.TaskIds);
    }

    [Fact]
    public void RemoveTask_WhenTaskDoesNotExist_DoesNothing()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        var nonExistingTaskId = TaskId.New();
        
        //Act
        var exception = Record.Exception(() =>user.RemoveTask(nonExistingTaskId));
        
        //Assert
        Assert.Null(exception);
        Assert.Empty(user.TaskIds);
    }

    [Fact]
    public void MarkEmailAsConfirmed_WhenCalled_SetsIsEmailConfirmedToTrue()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        
        //Act
        user.MarkEmailAsConfirmed();
        
        //Assert
        Assert.True(user.IsEmailConfirmed);
    }

    [Fact]
    public void ChangeEmail_WhenCalledWithValidData_UpdatesEmailAndResetsConfirmation()
    {
        //Arrange
        var user = UsersTestData.CreateOwner(isEmailConfirmed: true);
        const string newEmail = "newemail@test.com";
        
        //Act
        user.ChangeEmail(newEmail);
        
        //Assert
        Assert.Equal(newEmail, user.Email);
        Assert.False(user.IsEmailConfirmed);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ab")]
    [InlineData("@@@@test.com")]
    [InlineData("@@@@.test.com")]
    public void ChangeEmail_WhenCalledWithInvalidData_ThrowsInvalidEmailException(string invalidEmail)
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        
        //Act
        var exception = Record.Exception(() => user.ChangeEmail(invalidEmail));
        
        //Assert
        Assert.IsType<InvalidEmailException>(exception);
    }

    [Fact]
    public void MarkAsLoggedIn_WhenCalled_SetsLastLoggedInAt()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        var now = DateTime.UtcNow;
        
        //Act
        user.MarkAsLoggedIn(now);
        
        //Assert
        Assert.Equal(now, user.LastLoggedAtUtc);
    }
    
    [Fact]
    public void ChangeUsername_WhenCalledWithValidData_UpdatesUsername()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        const string newName = "NewName";
        
        //Act
        user.ChangeUsername(newName);
        
        //Assert
        Assert.Equal("NewName", user.Username);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ab")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefg")]
    public void ChangeUsername_WhenCalledWithInvalidData_ThrowsInvalidUsernameException(string invalidUsername)
    {
        // Arrange
        var user = UsersTestData.CreateOwner();

        // Act
        var exception = Record.Exception(() => user.ChangeUsername(invalidUsername));

        // Assert
        Assert.IsType<InvalidUsernameException>(exception);
    }
    
    [Fact]
    public void ChangeFullName_WhenCalledWithValidData_UpdatesFullName()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        const string newName = "NewName";
        
        //Act
        user.ChangeFullName(newName);
        
        //Assert
        Assert.Equal("NewName", user.FullName);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ab")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefg")]
    public void ChangeFullName_WhenCalledWithInvalidData_ThrowsInvalidFullNameException(string invalidFullName)
    {
        // Arrange
        var user = UsersTestData.CreateOwner();

        // Act
        var exception = Record.Exception(() => user.ChangeFullName(invalidFullName));

        // Assert
        Assert.IsType<InvalidFullNameException>(exception);
    }

    [Fact]
    public void ChangeProfileVisibility_WhenSetToFalse_SetsIsProfileVisibleForSharingToFalse()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        user.ChangeProfileVisibility(true); // explicit setup: avoid relying on constructor defaults
        
        //Act
        user.ChangeProfileVisibility(false);
        
        //Assert
        Assert.False(user.IsProfileVisibleForSharing);
    }
    
    [Fact]
    public void ChangeProfileVisibility_WhenSetToTrue_SetsIsProfileVisibleForSharingToTrue()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        user.ChangeProfileVisibility(false); 
        
        //Act
        user.ChangeProfileVisibility(true);
        
        //Assert
        Assert.True(user.IsProfileVisibleForSharing);
    }

    [Fact]
    public void ChangePassword_WhenCalledWithValidData_UpdatesPassword()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        const string newPassword = "NewPassword";
        
        //Act
        user.ChangePassword(newPassword);
        
        //Assert
        Assert.Equal("NewPassword", user.Password);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("x")]
    [InlineData("abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz")]
    public void ChangePassword_WhenCalledWithInvalidData_ThrowsInvalidPasswordException(string invalidPassword)
    {
        // Arrange
        var user = UsersTestData.CreateOwner();

        // Act
        var exception = Record.Exception(() => user.ChangePassword(invalidPassword));

        // Assert
        Assert.IsType<InvalidPasswordException>(exception);
    }
}
