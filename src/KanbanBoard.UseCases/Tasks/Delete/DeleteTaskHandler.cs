using KanbanBoard.Core.TaskAggregate;
using KanbanBoard.Core.TaskAggregate.Specifications;

namespace KanbanBoard.UseCases.Tasks.Delete;

public class DeleteTaskHandler(IRepository<KanbanTask> _repository)
  : ICommandHandler<DeleteTaskCommand, Result>
{
  public async ValueTask<Result> Handle(DeleteTaskCommand command,
    CancellationToken cancellationToken)
  {
    var spec = new TaskByIdSpec(command.TaskId);
    var task = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (task is null)
    {
      return Result.NotFound("Task not found");
    }

    await _repository.DeleteAsync(task, cancellationToken);

    return Result.Success();
  }
}
