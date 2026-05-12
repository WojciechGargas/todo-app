using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Configurations;

internal sealed class TaskShareConfiguration : IEntityTypeConfiguration<TaskShare>
{
    public void Configure(EntityTypeBuilder<TaskShare> builder)
    {
        builder.ToTable("task_shares");
        
        builder.HasKey(t => new { t.TaskId, t.UserId });
        
        builder.Property(t => t.TaskId)
            .HasColumnName("task_id")
            .HasConversion(id => id.Value, value => new TaskId(value))
            .IsRequired();
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(x => x.SharedByUserId)
            .HasColumnName("shared_by_user_id")
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(x => x.Permission)
            .HasColumnName("permission")
            .HasConversion<int>() // Read=1, Edit=2
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne<TodoTask>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.SharedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.TaskId);
    }
}