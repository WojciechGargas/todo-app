using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.DTO;
using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;
using Todo.Infrastructure.DAL;
using Todo.Tests.Integration.Infrastructure;
using Todo.Tests.Integration.Shared;

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

        var signInResponse = await IntegrationAuthHelper.SignInAsync
            (_backend, "owner@test.com", "Secret123!");
        Assert.Equal(HttpStatusCode.OK, signInResponse.StatusCode);

        var jwt = await signInResponse.Content.ReadFromJsonAsync<JwtDto>();
        Assert.NotNull(jwt);
        IntegrationAuthHelper.SetBearerToken(_backend, jwt.AccessToken);

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

        var signInResponse = await IntegrationAuthHelper.SignInAsync
            (_backend, "owner@test.com", "Secret123!");
        Assert.Equal(HttpStatusCode.OK, signInResponse.StatusCode);

        var jwt = await signInResponse.Content.ReadFromJsonAsync<JwtDto>();
        Assert.NotNull(jwt);
        IntegrationAuthHelper.SetBearerToken(_backend, jwt.AccessToken);

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

        var signInResponse = await IntegrationAuthHelper.SignInAsync(_backend, "owner@test.com", "Secret123!");
        Assert.Equal(HttpStatusCode.OK, signInResponse.StatusCode);

        var jwt = await signInResponse.Content.ReadFromJsonAsync<JwtDto>();
        Assert.NotNull(jwt);
        IntegrationAuthHelper.SetBearerToken(_backend, jwt.AccessToken);

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
}
