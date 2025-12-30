namespace KanbanBoard.Core.TaskAggregate.Events;

public class TaskMovedEvent : DomainEventBase
{
  public Guid TaskId { get; }
  public TaskStatus FromStatus { get; }
  public TaskStatus ToStatus { get; }
  public Guid BoardId { get; }

  public TaskMovedEvent(KanbanTask task, TaskStatus from, TaskStatus to)
  {
    TaskId = task.Id;
    BoardId = task.BoardId;
    FromStatus = from;
    ToStatus = to;
  }
}
