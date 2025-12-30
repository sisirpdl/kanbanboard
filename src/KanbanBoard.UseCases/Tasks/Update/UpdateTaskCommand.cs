namespace KanbanBoard.UseCases.Tasks.Update;

/// <summary>
/// Update a task's title and/or description.
/// </summary>
public record UpdateTaskCommand(
  Guid TaskId,
  string Title,
  string? Description = null
) : ICommand<Result>;
