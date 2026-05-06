using Microsoft.EntityFrameworkCore;
using Todo.Core.Entities;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;
using EmailAddress = Todo.Core.ValueObjects.Email;

namespace Todo.Infrastructure.DAL.Repositories;

public class UserRepository(TodoDbContext dbContext) : IUserRepository
{
    private readonly DbSet<User> _users = dbContext.Users;
    
    public Task<User?> GetUserByIdAsync(UserId userId)
        => _users.SingleOrDefaultAsync(u => u.UserId == userId);

    public Task<User?> GetUserByEmailAsync(EmailAddress email)
        =>  _users.SingleOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetUserByUsernameAsync(Username username)
        => _users.SingleOrDefaultAsync(u => u.Username == username);

    public async Task AddUserAsync(User user)
        => await _users.AddAsync(user);

    public async Task<bool> DeleteUserByIdAsync(UserId userId)
    {
        var deleted = await _users
            .Where(u => u.UserId == userId)
            .ExecuteDeleteAsync();

        return deleted > 0;
    }
}
