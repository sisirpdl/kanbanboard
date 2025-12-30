namespace KanbanBoard.UseCases.Tasks.Delete;

/// <summary>
/// Delete a task from the board.
/// </summary>
public record DeleteTaskCommand(Guid TaskId) : ICommand<Result>;
