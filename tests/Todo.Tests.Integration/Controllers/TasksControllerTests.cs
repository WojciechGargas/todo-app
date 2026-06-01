using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.DTO;
using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;
using Todo.Infrastructure.DAL;
using Todo.Tests.Integration.Infrastructure;
using Todo.Tests.Integration.Shared;
using Xunit.Sdk;

namespace Todo.Tests.Integration.Controllers;

public partial class TasksControllerTests(ApplicationWebFactory factory) : IClassFixture<ApplicationWebFactory>, IAsyncLifetime
{
    private readonly HttpClient _backend = factory.CreateClient();

    public Task InitializeAsync() => factory.ResetStateAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetTasks_WithoutToken_ReturnsUnauthorized()
    {
        //Arrange
        _backend.DefaultRequestHeaders.Authorization = null;

        //Act
        var response = await _backend.GetAsync("/tasks/tasks");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTasks_WithValidToken_ReturnsOnlyOwnedTasks()
    {
        // Arrange
        var (ownerTaskId, otherUserTaskId, sharedTaskId) = 
            await SeedMixedTasksAsync(TaskSharePermission.Read);

        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");

        // Act
        var response = await _backend.GetAsync("/tasks/tasks");
        var tasks = await response.Content.ReadFromJsonAsync<List<TodoTaskDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tasks);

        Assert.Contains(tasks, t => t.Id.Value == ownerTaskId);
        Assert.DoesNotContain(tasks, t => t.Id.Value == otherUserTaskId);
        Assert.DoesNotContain(tasks, t => t.Id.Value == sharedTaskId);
    }

    [Fact]
    public async Task GetTask_WhenOwner_ReturnsOk()
    {
        // Arrange
        var ownerTaskId = await SeedTaskForUserAsync(
            OwnerUserId,
            name: "Owner task",
            description: "Owned by owner");

        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");

        // Act
        var response = await _backend.GetAsync($"/tasks/{ownerTaskId}");
        var task = await response.Content.ReadFromJsonAsync<TodoTaskDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(task);
        Assert.Equal(ownerTaskId, task.Id.Value);
        Assert.Equal("Owner task", task.Name);
        Assert.Equal("Owned by owner", task.Description);
    }

    [Fact]
    public async Task GetTasksWithShared_WithValidToken_ReturnsOwnedAndSharedTasks()
    {
        // Arrange
        var (ownerTaskId, otherTaskId, sharedTaskId) = await SeedMixedTasksAsync(TaskSharePermission.Read);

        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");

        // Act
        var response = await _backend.GetAsync("/tasks/tasksWithShared");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tasks = await response.Content.ReadFromJsonAsync<List<TodoTaskDto>>();
        Assert.NotNull(tasks);

        // Assert
        Assert.Contains(tasks, t => t.Id.Value == ownerTaskId);
        Assert.Contains(tasks, t => t.Id.Value == sharedTaskId);
        Assert.DoesNotContain(tasks, t => t.Id.Value == otherTaskId);
    }
    
    [Fact]
    public async Task AddTask_WithValidRequest_ReturnsOkAndAddsTask()
    {
        // Arrange
        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");

        var request = new
        {
            name = "New Task",
            description = "Created from integration test"
        };
        
        //Act
        var response = await _backend.PostAsJsonAsync("/tasks/addTask", request);
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        using var scope = factory.Server.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        
        var exists = await db.TodoTasks.AnyAsync(t =>
            t.OwnerUserId == new UserId(OwnerUserId) &&
            t.TaskName == new TaskName("New Task") && 
            t.TaskDescription == new TaskDescription("Created from integration test"));
        
        Assert.True(exists);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz")]
    public async Task AddTask_WithInvalidName_ReturnsBadRequest(string invalidName)
    {
        // Arrange
        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");

        var request = new
        {
            name = invalidName,
            description = "Valid description"
        };
        
        //Act
        var response = await _backend.PostAsJsonAsync("/tasks/addTask", request);
        
        //Assert
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("invalid_todo_task_name", error.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz")]
    public async Task AddTask_WithInvalidDescription_ReturnsBadRequest(string invalidDescription)
    {
        // Arrange
        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");
        
        var request = new
        {
            name = "Valid name",
            description = invalidDescription
        };
        
        //Act
        var response = await _backend.PostAsJsonAsync("/tasks/addTask", request);
        
        //Assert
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("invalid_task_description", error.Code);
    }

    [Fact]
    public async Task UpdateTaskName_WhenOwner_ReturnsNoContentAndUpdatesTaskName()
    {
        //Arrange
        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");
        
        var ownerTaskId = await SeedTaskForUserAsync(
            OwnerUserId,
            name: "Owner task",
            description: "Owned by owner");

        var request = new
        {
            name = "Updated name"
        };
        
        // Act
        var response = await _backend.PatchAsJsonAsync($"/tasks/updateTask/{ownerTaskId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var task = await db.TodoTasks.FirstOrDefaultAsync(t =>t.TaskId == new TaskId(ownerTaskId));
        
        //Assert
        Assert.NotNull(task);
        Assert.Equal(new TaskName("Updated name"), task.TaskName);
    }

    [Fact]
    public async Task UpdateTaskName_WhenNotOwnerAndNotShared_ReturnsForbidden()
    {
        //Arrange
        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");
        
        var otherUsersTaskId = await SeedTaskForUserAsync(
            OtherUserId,
            name: "Other user task",
            description: "Owned by other user");

        var request = new
        {
            name = "Updated name"
        };
        
        //Act
        var response = await _backend.PatchAsJsonAsync($"/tasks/updateTask/{otherUsersTaskId}", request);
        
        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("forbidden", error.Code);
    }
    
    [Fact]
    public async Task UpdateTaskDescription_WhenOwner_ReturnsNoContentAndUpdatesTaskDescription()
    {
        //Arrange
        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");
        
        var ownerTaskId = await SeedTaskForUserAsync(
            OwnerUserId,
            name: "Owner task",
            description: "Owned by owner");

        var request = new
        {
            description = "Updated description"
        };
        
        // Act
        var response = await _backend.PatchAsJsonAsync($"/tasks/updateTask/{ownerTaskId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var task = await db.TodoTasks.FirstOrDefaultAsync(t =>t.TaskId == new TaskId(ownerTaskId));
        
        //Assert
        Assert.NotNull(task);
        Assert.Equal(new TaskDescription("Updated description"), task.TaskDescription);
    }
    
    [Fact]
    public async Task UpdateTaskDescription_WhenNotOwnerAndNotShared_ReturnsForbidden()
    {
        //Arrange
        await IntegrationAuthHelper.SignInAndSetBearerTokenAsync
            (_backend, "owner@test.com", "Secret123!");
        
        var otherUsersTaskId = await SeedTaskForUserAsync(
            OtherUserId,
            name: "Other user task",
            description: "Owned by other user");

        var request = new
        {
            description = "Updated description"
        };
        
        //Act
        var response = await _backend.PatchAsJsonAsync($"/tasks/updateTask/{otherUsersTaskId}", request);
        
        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("forbidden", error.Code);
    }
}
