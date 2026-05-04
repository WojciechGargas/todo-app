using Microsoft.AspNetCore.Mvc;
using Todo.Application.Abstractions;

namespace Todo.Api.Controllers;

[ApiController]
[Route("[controller]")]

public class AuthController(
    ICommandHandler<SignIn> signInHandler) : ControllerBase
{
    
}