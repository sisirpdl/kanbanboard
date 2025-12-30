namespace KanbanBoard.Core.TaskAggregate.Specifications;

public class TaskByIdSpec : Specification<KanbanTask>, ISingleResultSpecification<KanbanTask>
{
  public TaskByIdSpec(Guid taskId)
  {
    Query.Where(t => t.Id == taskId);
  }
}
