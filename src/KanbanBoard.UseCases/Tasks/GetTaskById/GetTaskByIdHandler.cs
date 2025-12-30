using KanbanBoard.Core.TaskAggregate;
using KanbanBoard.Core.TaskAggregate.Specifications;

namespace KanbanBoard.UseCases.Tasks.GetTaskById;

public class GetTaskByIdHandler(IReadRepository<KanbanTask> _repository)
  : IQueryHandler<GetTaskByIdQuery, Result<TaskDto>>
{
  public async ValueTask<Result<TaskDto>> Handle(GetTaskByIdQuery query,
    CancellationToken cancellationToken)
  {
    var spec = new TaskByIdSpec(query.TaskId);
    var task = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (task is null)
    {
      return Result.NotFound("Task not found");
    }

    var taskDto = new TaskDto(
      task.Id,
      task.Title,
      task.Description,
      task.Status.Name,
      task.Status.Value,
      task.BoardId,
      task.Position
    );

    return Result.Success(taskDto);
  }
}
