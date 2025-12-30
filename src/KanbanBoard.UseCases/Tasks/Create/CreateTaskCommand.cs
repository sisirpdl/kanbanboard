namespace KanbanBoard.UseCases.Tasks.Create;

/// <summary>
/// Create a new Task on a board.
/// </summary>
public record CreateTaskCommand(
  string Title,
  Guid BoardId,
  string? Description = null
) : ICommand<Result<Guid>>;
