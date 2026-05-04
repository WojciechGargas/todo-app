using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Configurations;

internal sealed class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
{
    private static readonly ValueComparer<List<UserId>> SharedWithComparer = new(
        (left, right) => left!.SequenceEqual(right!),
        value => value.Aggregate(0, (hash, id) => HashCode.Combine(hash, id.GetHashCode())),
        value => value.Select(x => new UserId(x.Value)).ToList());

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

        builder.Property<List<UserId>>("_sharedWithUserIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("shared_with_user_ids")
            .HasColumnType("text")
            .HasConversion(
                value => JsonSerializer.Serialize(value.Select(id => id.Value).ToList(), (JsonSerializerOptions?)null),
                value => string.IsNullOrWhiteSpace(value)
                    ? new List<UserId>()
                    : JsonSerializer.Deserialize<List<Guid>>(value, (JsonSerializerOptions?)null)!
                        .Select(id => new UserId(id))
                        .ToList())
            .Metadata.SetValueComparer(SharedWithComparer);
    }
}
