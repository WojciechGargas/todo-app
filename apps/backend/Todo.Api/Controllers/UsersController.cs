using Microsoft.AspNetCore.Mvc;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.Users.Queries.GetUser;
using Todo.Application.Users.Queries.GetUsers;
using Todo.Application.Users.Queries.GetUserWithTasks;

namespace Todo.Api.Controllers;

[ApiController]
[Route("[controller]")]

public class UsersController(
    IQueryHandler<GetUserQuery, UserDto> getUserHandler,
    IQueryHandler<GetUserWithTasksQuery, UserWithTasksDto> getUserWithTasksHandler,
    IQueryHandler<GetUsersQuery,  IEnumerable<UserDto>> getUsersHandler)
    : ControllerBase
{
    [HttpGet("{UserId:guid}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] Guid userId)
    {
        var user = await getUserHandler.HandleAsync(new GetUserQuery{ UserId = userId });
        
        return user;
    }

    [HttpGet("{UserId:guid}/tasks")]
    public async Task<ActionResult<UserWithTasksDto>> GetUserWithTasks([FromRoute] Guid userId)
    {
        var user = await getUserWithTasksHandler.HandleAsync(new GetUserWithTasksQuery{ UserId = userId });

        return user;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(GetUsersQuery query)
    {
        return Ok(await getUsersHandler.HandleAsync(query));
    }
}
