using KanbanBoard.Core.TaskAggregate.Events;

namespace KanbanBoard.Core.TaskAggregate.Handlers;

/// <summary>
/// Handles TaskMovedEvent to log task movements.
/// SignalR notifications will be added in the Web layer (Phase 4).
/// </summary>
public class TaskMovedEventHandler(ILogger<TaskMovedEventHandler> logger)
  : INotificationHandler<TaskMovedEvent>
{
  public ValueTask Handle(TaskMovedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation(
      "Task {TaskId} moved from {FromStatus} to {ToStatus} on Board {BoardId}",
      domainEvent.TaskId,
      domainEvent.FromStatus.Name,
      domainEvent.ToStatus.Name,
      domainEvent.BoardId
    );

    return ValueTask.CompletedTask;
  }
}
