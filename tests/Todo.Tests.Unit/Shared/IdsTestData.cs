using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Tests.Unit.Shared;

public static class IdsTestData
{
    public static readonly Guid UserOwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid UserCollaboratorId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid UserAdminId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid TaskOwnerTodoId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid TaskSharedTodoId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
}