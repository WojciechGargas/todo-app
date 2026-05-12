using Todo.Core.Entities;

namespace Todo.Core.Policies;

public interface ITaskChangePermissionPolicy
{
    bool CanChange(TodoTask task, User user);
}