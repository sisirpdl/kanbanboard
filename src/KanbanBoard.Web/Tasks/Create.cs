using System.ComponentModel.DataAnnotations;
using KanbanBoard.UseCases.Tasks.Create;
using KanbanBoard.Web.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KanbanBoard.Web.Tasks;

public class Create(IMediator mediator)
  : Endpoint<CreateTaskRequest,
          Results<Created<CreateTaskResponse>,
                          ValidationProblem,
                          ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(CreateTaskRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Create a new task";
      s.Description = "Creates a new task on a board with the provided title and optional description.";
      s.ExampleRequest = new CreateTaskRequest
      {
        BoardId = Guid.NewGuid(),
        Title = "Implement login feature",
        Description = "Add OAuth2 authentication"
      };

      s.Responses[201] = "Task created successfully";
      s.Responses[400] = "Invalid input data - validation errors";
      s.Responses[500] = "Internal server error";
    });

    Tags("Tasks");

    Description(builder => builder
      .Accepts<CreateTaskRequest>("application/json")
      .Produces<CreateTaskResponse>(201, "application/json")
      .ProducesProblem(400)
      .ProducesProblem(500));
  }

  public override async Task<Results<Created<CreateTaskResponse>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CreateTaskRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new CreateTaskCommand(request.Title!, request.BoardId, request.Description),
      cancellationToken);

    return result.ToCreatedResult(
      id => $"/Tasks/{id}",
      id => new CreateTaskResponse(id, request.Title!, request.BoardId));
  }
}

public class CreateTaskRequest
{
  public const string Route = "/Tasks";

  [Required]
  public Guid BoardId { get; set; }

  [Required]
  public string Title { get; set; } = string.Empty;

  public string? Description { get; set; }
}

public class CreateTaskValidator : Validator<CreateTaskRequest>
{
  public CreateTaskValidator()
  {
    RuleFor(x => x.BoardId)
      .NotEmpty()
      .WithMessage("BoardId is required.");

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

public class CreateTaskResponse(Guid id, string title, Guid boardId)
{
  public Guid Id { get; set; } = id;
  public string Title { get; set; } = title;
  public Guid BoardId { get; set; } = boardId;
}
