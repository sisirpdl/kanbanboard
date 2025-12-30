using KanbanBoard.Core.TaskAggregate;
using KanbanBoard.Core.TaskAggregate.Specifications;

namespace KanbanBoard.UseCases.Tasks.GetTasksByBoard;

public class GetTasksByBoardHandler(IReadRepository<KanbanTask> _repository)
  : IQueryHandler<GetTasksByBoardQuery, Result<List<TaskDto>>>
{
  public async ValueTask<Result<List<TaskDto>>> Handle(GetTasksByBoardQuery query,
    CancellationToken cancellationToken)
  {
    Core.TaskAggregate.TaskStatus? filterStatus = null;
    if (!string.IsNullOrEmpty(query.FilterStatus))
    {
      if (!Core.TaskAggregate.TaskStatus.TryFromName(query.FilterStatus, out filterStatus))
      {
        return Result.Invalid(new ValidationError(
          nameof(query.FilterStatus),
          $"Invalid status '{query.FilterStatus}'. Valid values are: ToDo, InProgress, Done"
        ));
      }
    }

    var spec = new TasksByBoardSpec(query.BoardId, filterStatus);
    var tasks = await _repository.ListAsync(spec, cancellationToken);

    var sortedTasks = tasks.OrderBy(t => t.Status.Value)
                           .ThenBy(t => t.Position)
                           .ToList();

    var taskDtos = sortedTasks.Select(t => new TaskDto(
      t.Id,
      t.Title,
      t.Description,
      t.Status.Name,
      t.Status.Value,
      t.BoardId,
      t.Position
    )).ToList();

    return Result.Success(taskDtos);
  }
}
