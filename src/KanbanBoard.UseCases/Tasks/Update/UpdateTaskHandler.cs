using KanbanBoard.Core.TaskAggregate;
using KanbanBoard.Core.TaskAggregate.Specifications;

namespace KanbanBoard.UseCases.Tasks.Update;

public class UpdateTaskHandler(IRepository<KanbanTask> _repository)
  : ICommandHandler<UpdateTaskCommand, Result>
{
  public async ValueTask<Result> Handle(UpdateTaskCommand command,
    CancellationToken cancellationToken)
  {
    var spec = new TaskByIdSpec(command.TaskId);
    var task = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (task is null)
    {
      return Result.NotFound("Task not found");
    }

    task.UpdateTitle(command.Title)
        .UpdateDescription(command.Description);

    await _repository.UpdateAsync(task, cancellationToken);

    return Result.Success();
  }
}
