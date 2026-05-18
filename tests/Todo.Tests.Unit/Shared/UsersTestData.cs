using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Tests.Unit.Shared;

public class UsersTestData
{
    private static readonly DateTime DefaultCreatedAtUtc =
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    
    public static User CreateOwner(
        Guid? userId = null,
        string? email = null,
        string? username = null,
        string? password = null,
        string? fullName = null,
        UserRole role = UserRole.User,
        DateTime? createdAtUtc = null,
        bool isEmailConfirmed = false)
        => new(
            new UserId(userId ?? IdsTestData.UserOwnerId),
            new Email(email ?? "owner@test.com"),
            new Username(username ?? "owner_user"),
            new Password(password ?? "Secret123!"),
            new FullName(fullName ?? "Owner User"),
            role,
            createdAtUtc ?? DefaultCreatedAtUtc,
            isEmailConfirmed);

    public static User CreateCollaborator(
        Guid? userId = null,
        string? email = null,
        string? username = null,
        string? password = null,
        string? fullName = null,
        DateTime? createdAtUtc = null,
        bool isEmailConfirmed = false)
        => new(
            new UserId(userId ?? IdsTestData.UserCollaboratorId),
            new Email(email ?? "collab@test.com"),
            new Username(username ?? "collab_user"),
            new Password(password ?? "Secret123!"),
            new FullName(fullName ?? "Collaborator User"),
            UserRole.User,
            createdAtUtc ?? DefaultCreatedAtUtc,
            isEmailConfirmed);

    public static User CreateAdmin(
        Guid? userId = null,
        string? email = null,
        string? username = null,
        string? password = null,
        string? fullName = null,
        DateTime? createdAtUtc = null,
        bool isEmailConfirmed = true)
        => new(
            new UserId(userId ?? IdsTestData.UserAdminId),
            new Email(email ?? "admin@test.com"),
            new Username(username ?? "admin_user"),
            new Password(password ?? "Secret123!"),
            new FullName(fullName ?? "Admin User"),
            UserRole.Admin,
            createdAtUtc ?? DefaultCreatedAtUtc,
            isEmailConfirmed);
}