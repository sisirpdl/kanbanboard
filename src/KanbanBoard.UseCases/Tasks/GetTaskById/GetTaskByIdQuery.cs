namespace KanbanBoard.UseCases.Tasks.GetTaskById;

/// <summary>
/// Get a specific task by its ID.
/// </summary>
public record GetTaskByIdQuery(Guid TaskId) : IQuery<Result<TaskDto>>;
