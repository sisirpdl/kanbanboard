namespace KanbanBoard.UseCases.Tasks.GetTasksByBoard;

/// <summary>
/// Get all tasks for a specific board, optionally filtered by status.
/// </summary>
public record GetTasksByBoardQuery(
  Guid BoardId,
  string? FilterStatus = null
) : IQuery<Result<List<TaskDto>>>;
