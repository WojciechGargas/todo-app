using Todo.Core.Entities;

namespace Todo.Core.Policies;

public interface IUserUpdatePolicy
{
    bool CanUpdate(User userToUpdate, User requestedBy);
}
