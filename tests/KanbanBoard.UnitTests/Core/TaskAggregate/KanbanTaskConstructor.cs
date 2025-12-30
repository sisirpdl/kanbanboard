using KanbanBoard.Core.TaskAggregate;

namespace KanbanBoard.UnitTests.Core.TaskAggregate;

public class KanbanTaskConstructor
{
  private readonly string _testTitle = "Test Task";
  private readonly Guid _testBoardId = Guid.NewGuid();
  private readonly string _testDescription = "Test description";

  [Fact]
  public void InitializesPropertiesCorrectly()
  {
    var task = new KanbanTask(_testTitle, _testBoardId, _testDescription);

    task.Title.ShouldBe(_testTitle);
    task.BoardId.ShouldBe(_testBoardId);
    task.Description.ShouldBe(_testDescription);
    task.Status.ShouldBe(TaskStatus.ToDo);
    task.Position.ShouldBe(0);
    task.Id.ShouldNotBe(Guid.Empty);
  }

  [Fact]
  public void ThrowsExceptionForNullTitle()
  {
    Should.Throw<ArgumentException>(() => new KanbanTask(null!, _testBoardId));
  }

  [Fact]
  public void ThrowsExceptionForEmptyTitle()
  {
    Should.Throw<ArgumentException>(() => new KanbanTask(string.Empty, _testBoardId));
  }

  [Fact]
  public void ThrowsExceptionForEmptyBoardId()
  {
    Should.Throw<ArgumentException>(() => new KanbanTask(_testTitle, Guid.Empty));
  }

  [Fact]
  public void AllowsNullDescription()
  {
    var task = new KanbanTask(_testTitle, _testBoardId, null);

    task.Description.ShouldBeNull();
  }
}
