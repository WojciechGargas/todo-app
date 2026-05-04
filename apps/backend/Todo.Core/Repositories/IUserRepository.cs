using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Core.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(UserId userId);
    Task<User?> GetUserByEmailAsync(Email email);
    Task<User?> GetUserByUsernameAsync(Username username);
    Task AddUserAsync(User user);
    Task<bool> DeleteUserByIdAsync(UserId  userId);
}