using KanbanBoard.UseCases.Tasks;
using KanbanBoard.UseCases.Tasks.GetTasksByBoard;
using KanbanBoard.Web.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KanbanBoard.Web.Tasks;

public class List(IMediator mediator)
  : Endpoint<GetTasksByBoardRequest, Ok<List<TaskDto>>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(GetTasksByBoardRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Get all tasks for a board";
      s.Description = "Retrieves all tasks on a specific board, optionally filtered by status. Tasks are ordered by status display order, then by position.";
      s.ExampleRequest = new GetTasksByBoardRequest
      {
        BoardId = Guid.NewGuid(),
        FilterStatus = "InProgress"
      };

      s.Responses[200] = "Tasks retrieved successfully";
    });

    Tags("Tasks");

    Description(builder => builder
      .Produces<List<TaskDto>>(200, "application/json"));
  }

  public override async Task<Ok<List<TaskDto>>>
    ExecuteAsync(GetTasksByBoardRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new GetTasksByBoardQuery(request.BoardId, request.FilterStatus),
      cancellationToken);

    return result.ToOkOnlyResult(tasks => tasks);
  }
}

public class GetTasksByBoardRequest
{
  public const string Route = "/Boards/{BoardId:guid}/tasks";

  public Guid BoardId { get; set; }

  public string? FilterStatus { get; set; }
}

public class GetTasksByBoardValidator : Validator<GetTasksByBoardRequest>
{
  public GetTasksByBoardValidator()
  {
    RuleFor(x => x.BoardId)
      .NotEmpty()
      .WithMessage("BoardId is required.");

    RuleFor(x => x.FilterStatus)
      .Must(status => status == null || status == "ToDo" || status == "InProgress" || status == "Done")
      .WithMessage("FilterStatus must be one of: ToDo, InProgress, Done")
      .When(x => !string.IsNullOrEmpty(x.FilterStatus));
  }
}
