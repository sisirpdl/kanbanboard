using KanbanBoard.Core.TaskAggregate.Events;

namespace KanbanBoard.Core.TaskAggregate;

public class KanbanTask : EntityBase<KanbanTask, Guid>, IAggregateRoot
{
  public string Title { get; private set; }
  public string? Description { get; private set; }
  public TaskStatus Status { get; private set; }
  public Guid BoardId { get; private set; }
  public int Position { get; private set; }

  // EF Core constructor
  private KanbanTask()
  {
    Title = string.Empty;
    Status = TaskStatus.ToDo;
  }

  public KanbanTask(string title, Guid boardId, string? description = null)
  {
    Guard.Against.NullOrEmpty(title, nameof(title));
    Guard.Against.Default(boardId, nameof(boardId));

    Id = Guid.NewGuid();
    Title = title;
    Description = description;
    BoardId = boardId;
    Status = TaskStatus.ToDo;
    Position = 0;
  }

  public KanbanTask UpdateTitle(string newTitle)
  {
    Guard.Against.NullOrEmpty(newTitle, nameof(newTitle));
    Title = newTitle;
    return this;
  }

  public KanbanTask UpdateDescription(string? newDescription)
  {
    Description = newDescription;
    return this;
  }

  public Result MoveTo(TaskStatus newStatus, int newPosition)
  {
    Guard.Against.Null(newStatus, nameof(newStatus));
    Guard.Against.Negative(newPosition, nameof(newPosition));

    // Validate transition
    if (!Status.CanTransitionTo(newStatus))
    {
      return Result.Invalid(
        new ValidationError(
          $"Cannot transition from {Status.Name} to {newStatus.Name}. " +
          "Tasks cannot skip from ToDo directly to Done."
        )
      );
    }

    var oldStatus = Status;
    Status = newStatus;
    Position = newPosition;

    // Raise domain event
    RegisterDomainEvent(new TaskMovedEvent(this, oldStatus, newStatus));

    return Result.Success();
  }

  public KanbanTask UpdatePosition(int newPosition)
  {
    Guard.Against.Negative(newPosition, nameof(newPosition));
    Position = newPosition;
    return this;
  }
}
