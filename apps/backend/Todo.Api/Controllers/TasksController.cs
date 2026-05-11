using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Auth;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.TodoTasks.Commands.AddTask;
using Todo.Application.TodoTasks.Commands.DeleteTask;
using Todo.Application.TodoTasks.Commands.UpdateTask;
using Todo.Application.TodoTasks.Queries.GetTask;
using Todo.Application.TodoTasks.Queries.GetTasks;
using Todo.Core.ValueObjects;

namespace Todo.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TasksController(
    ICommandHandler<AddTaskCommand> addTaskCommandHandler,
    ICommandHandler<DeleteTaskCommand> deleteTaskCommandHandler,
    ICommandHandler<UpdateTaskCommand> updateTaskCommandHandler,
    IQueryHandler<GetTaskQuery, TodoTaskDto> getTaskHandler,
    IQueryHandler<GetTasksQuery, IReadOnlyList<TodoTaskDto>> getTasksHandler)
    : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoTaskDto>> GetTask([FromRoute] Guid id)
    {
        var userId = User.GetUserIdOrThrow();
        var activity = await getTaskHandler.HandleAsync(new GetTaskQuery
        {
            UserId = userId,
            Id = new TaskId(id)
        });

        return activity;
    }

    [HttpGet("tasks")]
    public async Task<ActionResult<IReadOnlyList<TodoTaskDto>>> GetTasks()
    {
        var userId = User.GetUserIdOrThrow();

        var tasks = await getTasksHandler.HandleAsync(new GetTasksQuery
        {
            UserId = userId
        });

        return Ok(tasks);
    }
    
    [HttpPost("addTask")]
    public async Task<ActionResult> AddTask([FromBody] AddTaskRequest request)
    {
        var userId = User.GetUserIdOrThrow();
        var id = TaskId.New();

        var command = new AddTaskCommand(
            id,
            userId,
            request.Name,
            request.Description
        );

        await addTaskCommandHandler.HandleAsync(command);
        
        return Ok();
    }

    [HttpPatch("updateTask/{id:guid}")]
    public async Task<ActionResult> UpdateTask([FromRoute] Guid id, [FromBody] UpdateTaskRequest request)
    {
        var userId = User.GetUserIdOrThrow();
        var command = new UpdateTaskCommand(
            new TaskId(id),
            userId,
            request.Name,
            request.Description,
            request.IsCompleted);
        
        await updateTaskCommandHandler.HandleAsync(command);
        
        return NoContent();
    }

    [HttpDelete("deleteTask/{id:guid}")]
    public async Task<ActionResult> DeleteTask([FromRoute] Guid id)
    {
        var userId = User.GetUserIdOrThrow();
        var command = new DeleteTaskCommand(userId, new TaskId(id));
        
        await deleteTaskCommandHandler.HandleAsync(command);
        
        return NoContent();
    }

    [HttpPost("/users/{userId:guid}/addTask")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult> AddTaskForUser([FromRoute] Guid userId, [FromBody] AddTaskRequest request)
    {
        var id = TaskId.New();
        
        var command = new AddTaskCommand(
            id,
            userId,
            request.Name,
            request.Description
        );
        
        await addTaskCommandHandler.HandleAsync(command);
        
        return Ok();
    }
}
