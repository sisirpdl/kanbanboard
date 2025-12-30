using System.ComponentModel.DataAnnotations;
using KanbanBoard.UseCases.Tasks.MoveTask;
using KanbanBoard.Web.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KanbanBoard.Web.Tasks;

public class MoveTask(IMediator mediator)
  : Endpoint<MoveTaskRequest,
          Results<Ok, NotFound, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Patch(MoveTaskRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Move a task to a different status/column";
      s.Description = "Updates the task's status and position. Validates that status transitions follow business rules (e.g., cannot skip from ToDo to Done).";
      s.ExampleRequest = new MoveTaskRequest
      {
        TaskId = Guid.NewGuid(),
        NewStatus = "InProgress",
        NewPosition = 0
      };

      s.Responses[200] = "Task moved successfully";
      s.Responses[404] = "Task not found";
      s.Responses[400] = "Invalid input or business rule violation";
    });

    Tags("Tasks");

    Description(builder => builder
      .Accepts<MoveTaskRequest>("application/json")
      .Produces(200)
      .ProducesProblem(404)
      .ProducesValidationProblem(400)
      .ProducesProblem(400));
  }

  public override async Task<Results<Ok, NotFound, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(MoveTaskRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new MoveTaskCommand(request.TaskId, request.NewStatus!, request.NewPosition),
      cancellationToken);

    return result.ToCommandResult();
  }
}

public class MoveTaskRequest
{
  public const string Route = "/Tasks/{TaskId:guid}/move";

  public Guid TaskId { get; set; }

  [Required]
  public string NewStatus { get; set; } = string.Empty;

  [Required]
  public int NewPosition { get; set; }
}

public class MoveTaskValidator : Validator<MoveTaskRequest>
{
  public MoveTaskValidator()
  {
    RuleFor(x => x.TaskId)
      .NotEmpty()
      .WithMessage("TaskId is required.");

    RuleFor(x => x.NewStatus)
      .NotEmpty()
      .WithMessage("NewStatus is required.")
      .Must(status => status == "ToDo" || status == "InProgress" || status == "Done")
      .WithMessage("NewStatus must be one of: ToDo, InProgress, Done");

    RuleFor(x => x.NewPosition)
      .GreaterThanOrEqualTo(0)
      .WithMessage("NewPosition must be non-negative.");
  }
}
