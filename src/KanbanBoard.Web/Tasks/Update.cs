using System.ComponentModel.DataAnnotations;
using KanbanBoard.UseCases.Tasks.Update;
using KanbanBoard.Web.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KanbanBoard.Web.Tasks;

public class Update(IMediator mediator)
  : Endpoint<UpdateTaskRequest,
          Results<Ok, NotFound, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Put(UpdateTaskRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Update a task";
      s.Description = "Updates a task's title and/or description.";
      s.ExampleRequest = new UpdateTaskRequest
      {
        TaskId = Guid.NewGuid(),
        Title = "Updated task title",
        Description = "Updated description"
      };

      s.Responses[200] = "Task updated successfully";
      s.Responses[404] = "Task not found";
      s.Responses[400] = "Invalid input data";
    });

    Tags("Tasks");

    Description(builder => builder
      .Accepts<UpdateTaskRequest>("application/json")
      .Produces(200)
      .ProducesProblem(404)
      .ProducesValidationProblem(400));
  }

  public override async Task<Results<Ok, NotFound, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(UpdateTaskRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new UpdateTaskCommand(request.TaskId, request.Title!, request.Description),
      cancellationToken);

    return result.ToCommandResult();
  }
}

public class UpdateTaskRequest
{
  public const string Route = "/Tasks/{TaskId:guid}";

  public Guid TaskId { get; set; }

  [Required]
  public string Title { get; set; } = string.Empty;

  public string? Description { get; set; }
}

public class UpdateTaskValidator : Validator<UpdateTaskRequest>
{
  public UpdateTaskValidator()
  {
    RuleFor(x => x.TaskId)
      .NotEmpty()
      .WithMessage("TaskId is required.");

    RuleFor(x => x.Title)
      .NotEmpty()
      .WithMessage("Title is required.")
      .MinimumLength(1)
      .MaximumLength(200);

    RuleFor(x => x.Description)
      .MaximumLength(2000)
      .When(x => !string.IsNullOrEmpty(x.Description));
  }
}
