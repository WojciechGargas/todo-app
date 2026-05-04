using Microsoft.AspNetCore.Mvc;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.Quaries;
using Todo.Core.ValueObjects;

namespace Todo.Api.Controllers;

[ApiController]
[Route("[controller]")]

public class UsersController(
    IQueryHandler<GetUser, UserDto> getUserHandler,
    IQueryHandler<GetUsers,  IEnumerable<UserDto>> getUsersHandler) 
    : ControllerBase
{
    [HttpGet("{UserId:guid}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] Guid userId)
    {
        var user = await getUserHandler.HandleAsync(new GetUser{ UserId = userId });
        if (user is null)
            return NotFound();
        
        return user;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromRoute] GetUsers query)
    {
        return Ok(await getUsersHandler.HandleAsync(query));
    }
}