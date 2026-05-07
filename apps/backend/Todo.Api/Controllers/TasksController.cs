using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Auth;
using Todo.Application.Abstractions;
using Todo.Application.Commands.TodoTaskCommands;
using Todo.Application.DTO;
using Todo.Application.Quaries;
using Todo.Core.ValueObjects;

namespace Todo.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TasksController(
    ICommandHandler<AddTask> addTaskCommandHandler,
    IQueryHandler<GetTask, TodoTaskDto>  getTaskHandler)
    : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoTaskDto>> GetTask([FromRoute] Guid id)
    {
        var userId = User.GetUserIdOrThrow();
        var activity = await getTaskHandler.HandleAsync(new GetTask
        {
            UserId = userId,
            Id = new TaskId(id)
        });

        return activity;
    }
    
    [HttpPost("addTask")]
    public async Task<ActionResult> AddTask([FromBody] AddTaskRequest request)
    {
        var userId = User.GetUserIdOrThrow();
        var id = TaskId.New();

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
        var id = TaskId.New();
        
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
