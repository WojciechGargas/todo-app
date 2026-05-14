using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Auth;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.Users.Queries.SearchVisibleUsers;

namespace Todo.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]

public class UsersController(
    IQueryHandler<SearchVisibleUsersQuery, IReadOnlyList<UserShareCandidateDto>> searchVisibleUsersHandler) 
    : ControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<UserShareCandidateDto>>> SearchVisibleUser(
        [FromQuery] string? query)
    {
        var userId = User.GetUserIdOrThrow();

        var users = await searchVisibleUsersHandler.HandleAsync(new SearchVisibleUsersQuery
        {
            RequestedUserId = userId,
            Query = query
        });
        
        return Ok(users);
    }
}
