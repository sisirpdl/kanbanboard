namespace KanbanBoard.UseCases.Tasks.MoveTask;

/// <summary>
/// Move a task to a different status/column and position.
/// </summary>
public record MoveTaskCommand(
  Guid TaskId,
  string NewStatus,
  int NewPosition
) : ICommand<Result>;
