using Todo.Core.ValueObjects;

namespace Todo.Core.Entities;

public class TodoTask(
    TaskId taskId,
    UserId ownerUserId,
    TaskName taskName,
    TaskDescription taskDescription)
{
    public TaskId TaskId {get; private set;} = taskId;
    public UserId OwnerUserId {get; private set;} = ownerUserId;
    public TaskName TaskName { get; private set; } = taskName;
    public TaskDescription TaskDescription { get; private set; } = taskDescription;
    public bool IsCompleted { get; private set; }


    public void ChangeName(string newName)
        => TaskName = new TaskName(newName);
    
    public void ChangeDescription(string newDescription)
        => TaskDescription = new TaskDescription(newDescription);

    public void MarkAsCompleted()
    {
        if(IsCompleted)  return;
        IsCompleted = true;
    }
    
    public void MarkAsUncompleted()
    {
        if(!IsCompleted)  return;
        IsCompleted = false;
    }
}