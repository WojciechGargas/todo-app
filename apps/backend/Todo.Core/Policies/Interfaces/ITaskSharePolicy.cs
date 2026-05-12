using Todo.Core.Entities;

namespace Todo.Core.Policies;

public interface ITaskSharePolicy
{
    bool CanShare(TodoTask task, User user);
}