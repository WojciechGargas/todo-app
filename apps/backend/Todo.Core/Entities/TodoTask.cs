using Todo.Core.ValueObjects;

namespace Todo.Core.Entities;

public class TodoTask
{
    public TaskId TaskId {get; private set;}
    public UserId OwnerUserId {get; private set;}
    public TaskName TaskName { get; private set; }
    public TaskDescription TaskDescription { get; private set; }
    public bool IsCompleted { get; private set; }

    public TodoTask(TaskId taskId, UserId ownerUserId, TaskName taskName,
        TaskDescription taskDescription)
    {
        TaskId = taskId;
        OwnerUserId = ownerUserId;
        TaskName = taskName;
        TaskDescription = taskDescription;
    }
    
    
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