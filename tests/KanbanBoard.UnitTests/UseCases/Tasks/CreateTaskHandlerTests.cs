using KanbanBoard.Core.TaskAggregate;
using KanbanBoard.UseCases.Tasks.Create;
using NSubstitute;

namespace KanbanBoard.UnitTests.UseCases.Tasks;

public class CreateTaskHandlerTests
{
  private readonly IRepository<KanbanTask> _repository;
  private readonly CreateTaskHandler _handler;

  public CreateTaskHandlerTests()
  {
    _repository = Substitute.For<IRepository<KanbanTask>>();
    _handler = new CreateTaskHandler(_repository);
  }

  [Fact]
  public async Task CreatesTaskSuccessfully()
  {
    // Arrange
    var boardId = Guid.NewGuid();
    var command = new CreateTaskCommand("Test Task", boardId, "Test description");

    _repository.AddAsync(Arg.Any<KanbanTask>(), Arg.Any<CancellationToken>())
      .Returns(callInfo =>
      {
        var task = callInfo.Arg<KanbanTask>();
        return task;
      });

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.ShouldNotBe(Guid.Empty);

    await _repository.Received(1).AddAsync(
      Arg.Is<KanbanTask>(t =>
        t.Title == "Test Task" &&
        t.BoardId == boardId &&
        t.Description == "Test description" &&
        t.Status == TaskStatus.ToDo),
      Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task CreatesTaskWithoutDescription()
  {
    // Arrange
    var boardId = Guid.NewGuid();
    var command = new CreateTaskCommand("Test Task", boardId);

    _repository.AddAsync(Arg.Any<KanbanTask>(), Arg.Any<CancellationToken>())
      .Returns(callInfo => callInfo.Arg<KanbanTask>());

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();

    await _repository.Received(1).AddAsync(
      Arg.Is<KanbanTask>(t => t.Description == null),
      Arg.Any<CancellationToken>());
  }
}
