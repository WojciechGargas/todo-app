using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Configurations;

internal sealed class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
{
    public void Configure(EntityTypeBuilder<TodoTask> builder)
    {
        builder.ToTable("todo_tasks");

        builder.HasKey(x => x.TaskId);

        builder.Property(x => x.TaskId)
            .HasConversion(id => id.Value, value => new TaskId(value));

        builder.Property(x => x.OwnerUserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(x => x.TaskName)
            .HasConversion(v => v.Value, value => new TaskName(value))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.TaskDescription)
            .HasConversion(v => v.Value, value => new TaskDescription(value))
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.IsCompleted).IsRequired();
    }
}
