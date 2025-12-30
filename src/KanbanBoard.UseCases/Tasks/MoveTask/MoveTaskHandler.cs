using KanbanBoard.Core.TaskAggregate;
using KanbanBoard.Core.TaskAggregate.Specifications;

namespace KanbanBoard.UseCases.Tasks.MoveTask;

public class MoveTaskHandler(IRepository<KanbanTask> _repository)
  : ICommandHandler<MoveTaskCommand, Result>
{
  public async ValueTask<Result> Handle(MoveTaskCommand command,
    CancellationToken cancellationToken)
  {
    // Find the task
    var spec = new TaskByIdSpec(command.TaskId);
    var task = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (task is null)
    {
      return Result.NotFound("Task not found");
    }

    // Parse the status
    if (!Core.TaskAggregate.TaskStatus.TryFromName(command.NewStatus, out var newStatus))
    {
      return Result.Invalid(new ValidationError(
        nameof(command.NewStatus),
        $"Invalid status '{command.NewStatus}'. Valid values are: ToDo, InProgress, Done"
      ));
    }

    // Attempt to move the task (this validates the transition)
    var moveResult = task.MoveTo(newStatus, command.NewPosition);
    if (!moveResult.IsSuccess)
    {
      return moveResult;
    }

    // Save changes
    await _repository.UpdateAsync(task, cancellationToken);

    return Result.Success();
  }
}
