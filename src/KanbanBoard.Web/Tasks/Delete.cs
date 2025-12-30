using KanbanBoard.UseCases.Tasks.Delete;
using KanbanBoard.Web.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KanbanBoard.Web.Tasks;

public class Delete(IMediator mediator)
  : Endpoint<DeleteTaskRequest,
          Results<NoContent, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Delete(DeleteTaskRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Delete a task";
      s.Description = "Permanently removes a task from the board.";
      s.ExampleRequest = new DeleteTaskRequest { TaskId = Guid.NewGuid() };

      s.Responses[204] = "Task deleted successfully";
      s.Responses[404] = "Task not found";
      s.Responses[400] = "Delete operation failed";
    });

    Tags("Tasks");

    Description(builder => builder
      .Produces(204)
      .ProducesProblem(404)
      .ProducesProblem(400));
  }

  public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
    ExecuteAsync(DeleteTaskRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new DeleteTaskCommand(request.TaskId),
      cancellationToken);

    return result.ToDeleteResult();
  }
}

public class DeleteTaskRequest
{
  public const string Route = "/Tasks/{TaskId:guid}";

  public Guid TaskId { get; set; }
}

public class DeleteTaskValidator : Validator<DeleteTaskRequest>
{
  public DeleteTaskValidator()
  {
    RuleFor(x => x.TaskId)
      .NotEmpty()
      .WithMessage("TaskId is required.");
  }
}
