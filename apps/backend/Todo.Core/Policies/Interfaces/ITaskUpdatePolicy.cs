using Todo.Core.Entities;

namespace Todo.Core.Policies;

public interface ITaskUpdatePolicy
{
    bool CanUpdate(TodoTask task, User user, TaskShare? share);
}