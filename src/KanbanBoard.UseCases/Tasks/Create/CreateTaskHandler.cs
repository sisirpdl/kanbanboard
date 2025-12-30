using KanbanBoard.Core.TaskAggregate;

namespace KanbanBoard.UseCases.Tasks.Create;

public class CreateTaskHandler(IRepository<KanbanTask> _repository)
  : ICommandHandler<CreateTaskCommand, Result<Guid>>
{
  public async ValueTask<Result<Guid>> Handle(CreateTaskCommand command,
    CancellationToken cancellationToken)
  {
    var newTask = new KanbanTask(command.Title, command.BoardId, command.Description);

    var createdTask = await _repository.AddAsync(newTask, cancellationToken);

    return createdTask.Id;
  }
}
