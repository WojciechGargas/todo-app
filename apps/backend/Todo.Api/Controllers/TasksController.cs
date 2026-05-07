using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Auth;
using Todo.Application.Abstractions;
using Todo.Application.Commands.TodoTaskCommands;
using Todo.Core.ValueObjects;

namespace Todo.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TasksController(
    ICommandHandler<AddTask> addTaskCommandHandler)
    : ControllerBase
{
    [HttpPost("addTask")]
    public async Task<ActionResult> AddTask([FromBody] AddTaskRequest request)
    {
        var userId = User.GetUserIdOrThrow();
        var id = new TaskId();

        var command = new AddTask(
            id,
            userId,
            request.Name,
            request.Description
        );

        await addTaskCommandHandler.HandleAsync(command);
        return Ok();
    }

    [HttpPost("/users/{userId:guid}/tasks")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult> AddTaskForUser([FromRoute] Guid userId, [FromBody] AddTaskRequest request)
    {
        var id = new TaskId();
        
        var command = new AddTask(
            id,
            userId,
            request.Name,
            request.Description
        );
        
        await addTaskCommandHandler.HandleAsync(command);
        return Ok();
    }
}
