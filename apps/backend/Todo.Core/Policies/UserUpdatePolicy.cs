using Todo.Core.Entities;
using Todo.Core.Enums;

namespace Todo.Core.Policies;

public class UserUpdatePolicy : IUserUpdatePolicy
{
    public bool CanUpdate(User userToUpdate, User requestedBy)
        => requestedBy.Role == UserRole.Admin || requestedBy.UserId == userToUpdate.UserId;
}
