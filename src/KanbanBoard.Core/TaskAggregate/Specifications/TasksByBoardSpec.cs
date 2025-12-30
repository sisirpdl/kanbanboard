namespace KanbanBoard.Core.TaskAggregate.Specifications;

public class TasksByBoardSpec : Specification<KanbanTask>
{
  public TasksByBoardSpec(Guid boardId, TaskStatus? filterStatus = null)
  {
    Query.Where(t => t.BoardId == boardId);

    if (filterStatus != null)
    {
      Query.Where(t => t.Status == filterStatus);
    }

    Query.OrderBy(t => t.Position);
  }
}
