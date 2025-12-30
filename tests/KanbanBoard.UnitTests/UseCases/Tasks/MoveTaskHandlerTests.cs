using KanbanBoard.Core.TaskAggregate;
using KanbanBoard.Core.TaskAggregate.Specifications;
using KanbanBoard.UseCases.Tasks.MoveTask;
using NSubstitute;

namespace KanbanBoard.UnitTests.UseCases.Tasks;

public class MoveTaskHandlerTests
{
  private readonly IRepository<KanbanTask> _repository;
  private readonly MoveTaskHandler _handler;

  public MoveTaskHandlerTests()
  {
    _repository = Substitute.For<IRepository<KanbanTask>>();
    _handler = new MoveTaskHandler(_repository);
  }

  [Fact]
  public async Task MovesTaskSuccessfully()
  {
    // Arrange
    var task = new KanbanTask("Test Task", Guid.NewGuid());
    var taskId = task.Id;
    _repository.FirstOrDefaultAsync(Arg.Any<TaskByIdSpec>(), Arg.Any<CancellationToken>())
      .Returns(task);

    var command = new MoveTaskCommand(taskId, "InProgress", 5);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    task.Status.ShouldBe(TaskStatus.InProgress);
    task.Position.ShouldBe(5);
    await _repository.Received(1).UpdateAsync(task, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ReturnsNotFoundWhenTaskDoesNotExist()
  {
    // Arrange
    _repository.FirstOrDefaultAsync(Arg.Any<TaskByIdSpec>(), Arg.Any<CancellationToken>())
      .Returns((KanbanTask?)null);

    var command = new MoveTaskCommand(Guid.NewGuid(), "InProgress", 0);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(ResultStatus.NotFound);
  }

  [Fact]
  public async Task ReturnsInvalidForInvalidStatus()
  {
    // Arrange
    var task = new KanbanTask("Test Task", Guid.NewGuid());
    _repository.FirstOrDefaultAsync(Arg.Any<TaskByIdSpec>(), Arg.Any<CancellationToken>())
      .Returns(task);

    var command = new MoveTaskCommand(task.Id, "InvalidStatus", 0);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(ResultStatus.Invalid);
    result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Invalid status"));
  }

  [Fact]
  public async Task ReturnsInvalidForInvalidTransition()
  {
    // Arrange
    var task = new KanbanTask("Test Task", Guid.NewGuid());
    _repository.FirstOrDefaultAsync(Arg.Any<TaskByIdSpec>(), Arg.Any<CancellationToken>())
      .Returns(task);

    var command = new MoveTaskCommand(task.Id, "Done", 0); // Can't go from ToDo to Done

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(ResultStatus.Invalid);
  }
}
