using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Auth;
using Todo.Api.Exceptions;
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
    public async Task<ActionResult> AddTask(AddTaskRequest request)
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
        return Created();
    }
    
    
}