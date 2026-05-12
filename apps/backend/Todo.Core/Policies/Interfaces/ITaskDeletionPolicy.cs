using Todo.Core.Entities;

namespace Todo.Core.Policies;

public interface ITaskDeletionPolicy
{
    bool CanDelete(TodoTask task, User user);
}