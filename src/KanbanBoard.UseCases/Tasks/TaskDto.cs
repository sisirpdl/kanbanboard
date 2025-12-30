namespace KanbanBoard.UseCases.Tasks;

public record TaskDto(
  Guid Id,
  string Title,
  string? Description,
  string Status,
  int StatusValue,
  Guid BoardId,
  int Position
);
