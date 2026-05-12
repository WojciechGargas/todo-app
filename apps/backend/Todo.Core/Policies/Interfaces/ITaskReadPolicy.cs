using Todo.Core.Entities;

namespace Todo.Core.Policies;

public interface ITaskReadPolicy
{
    bool CanRead(TodoTask task, User user, TaskShare? share);
}