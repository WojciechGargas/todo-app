using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    private static readonly ValueComparer<IReadOnlyList<TaskId>> TaskIdsComparer = new(
        (left, right) => left!.SequenceEqual(right!),
        value => value.Aggregate(0, (hash, id) => HashCode.Combine(hash, id.GetHashCode())),
        value => value.ToList());

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.UserId)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.Property(x => x.Email)
            .HasConversion(v => v.Value, value => new Email(value))
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(x => x.Username)
            .HasConversion(v => v.Value, value => new Username(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Password)
            .HasConversion(v => v.Value, value => new Password(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasConversion(v => v.Value, value => new FullName(value))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Role).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.LastLoggedAtUtc);

        builder.Property(x => x.TaskIds)
            .HasField("_taskIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("task_ids")
            .HasColumnType("text")
            .HasConversion(
                value => JsonSerializer.Serialize(value.Select(id => id.Value).ToList(), (JsonSerializerOptions?)null),
                value => string.IsNullOrWhiteSpace(value)
                    ? (IReadOnlyList<TaskId>)new List<TaskId>()
                    : JsonSerializer.Deserialize<List<Guid>>(value, (JsonSerializerOptions?)null)!
                        .Select(id => new TaskId(id))
                        .ToList())
            .Metadata.SetValueComparer(TaskIdsComparer);
    }
}
