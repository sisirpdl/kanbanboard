using KanbanBoard.UseCases.Tasks;
using KanbanBoard.UseCases.Tasks.GetTaskById;
using KanbanBoard.Web.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KanbanBoard.Web.Tasks;

public class GetById(IMediator mediator)
  : Endpoint<GetTaskByIdRequest,
          Results<Ok<TaskDto>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(GetTaskByIdRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Get a task by ID";
      s.Description = "Retrieves a specific task by its unique identifier.";
      s.ExampleRequest = new GetTaskByIdRequest { TaskId = Guid.NewGuid() };

      s.Responses[200] = "Task retrieved successfully";
      s.Responses[404] = "Task not found";
    });

    Tags("Tasks");

    Description(builder => builder
      .Produces<TaskDto>(200, "application/json")
      .ProducesProblem(404));
  }

  public override async Task<Results<Ok<TaskDto>, NotFound, ProblemHttpResult>>
    ExecuteAsync(GetTaskByIdRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new GetTaskByIdQuery(request.TaskId),
      cancellationToken);

    return result.ToGetByIdResult(task => task);
  }
}

public class GetTaskByIdRequest
{
  public const string Route = "/Tasks/{TaskId:guid}";

  public Guid TaskId { get; set; }
}

public class GetTaskByIdValidator : Validator<GetTaskByIdRequest>
{
  public GetTaskByIdValidator()
  {
    RuleFor(x => x.TaskId)
      .NotEmpty()
      .WithMessage("TaskId is required.");
  }
}
