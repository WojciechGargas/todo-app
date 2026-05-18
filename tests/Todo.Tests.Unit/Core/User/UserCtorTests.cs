using Todo.Core.Enums;
using Todo.Core.ValueObjects;
using Todo.Tests.Unit.Shared;

namespace Todo.Tests.Unit.Core;

public class UserCtorTests
{
    [Fact]
    public void Ctor_WhenDataIsValid_SetsAllProperties()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        
        //Act
            // ctor executed in CreateOwner();
            
        //Assert
        Assert.Multiple(
            () => Assert.Equal(IdsTestData.UserOwnerId, (Guid)user.UserId),
            () => Assert.Equal("owner@test.com", (string)user.Email),
            () => Assert.Equal("owner_user", (string)user.Username),
            () => Assert.Equal("Secret123!", (string)user.Password),
            () => Assert.Equal("Owner User", (string)user.FullName),
            () => Assert.Equal(UserRole.User, user.Role),
            () => Assert.Equal(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), user.CreatedAt),
            () => Assert.False(user.IsEmailConfirmed),
            () => Assert.True(user.IsProfileVisibleForSharing),
            () => Assert.Null(user.LastLoggedAtUtc),
            () => Assert.Empty(user.TaskIds)
        );
    }
    
    [Fact]
    public void Ctor_SetsProfileVisibilityToTrueByDefault()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        
        //Act
        
        //Assert
        Assert.True(user.IsProfileVisibleForSharing);
    }

    [Fact]
    public void Ctor_SetsIsEmailConfirmedToFalseByDefault()
    {
        //Arrange
        var usuer = UsersTestData.CreateOwner();
        
        //Act
        
        //Assert
        Assert.False(usuer.IsEmailConfirmed);
    }

    [Fact]
    public void Ctor_InitializesTaskIdsAsEmptyList()
    {
        //Arrange
        var user = UsersTestData.CreateOwner();
        
        //Act
        
        //Assert
        Assert.Empty(user.TaskIds);
    }
}