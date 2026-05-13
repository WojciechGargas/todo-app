using Microsoft.EntityFrameworkCore;
using Todo.Core.Entities;
using Todo.Infrastructure.Auth;

namespace Todo.Infrastructure.DAL;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TodoTask> TodoTasks => Set<TodoTask>();
    public DbSet<TaskShare> TaskShares => Set<TaskShare>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}