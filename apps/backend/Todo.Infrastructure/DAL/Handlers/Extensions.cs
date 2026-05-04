using Todo.Application.DTO;
using Todo.Core.Entities;

namespace Todo.Infrastructure.DAL.Handlers;

internal static class Extensions
{
    public static UserDto AsDto(this User entity)
    {
        return new UserDto
        {
            Id = entity.UserId,
            Email = entity.Email,
            UserName = entity.Username,
            Role = entity.Role
        };
    }
}