namespace KanbanBoard.Core.TaskAggregate;

public sealed class TaskStatus : SmartEnum<TaskStatus>
{
  public static readonly TaskStatus ToDo = new(nameof(ToDo), 1, 0);
  public static readonly TaskStatus InProgress = new(nameof(InProgress), 2, 1);
  public static readonly TaskStatus Done = new(nameof(Done), 3, 2);

  public int DisplayOrder { get; }

  private TaskStatus(string name, int value, int displayOrder) : base(name, value)
  {
    DisplayOrder = displayOrder;
  }

  public bool CanTransitionTo(TaskStatus target)
  {
    // Business rule: Can't skip InProgress when moving from ToDo to Done
    if (this == ToDo && target == Done) return false;

    // All other transitions are allowed
    return true;
  }
}
