using KanbanBoard.Core.TaskAggregate;
using KanbanBoard.Core.TaskAggregate.Events;

namespace KanbanBoard.UnitTests.Core.TaskAggregate;

public class KanbanTaskMoveTo
{
  private readonly string _testTitle = "Test Task";
  private readonly Guid _testBoardId = Guid.NewGuid();

  [Fact]
  public void MovesTaskToNewStatusSuccessfully()
  {
    var task = new KanbanTask(_testTitle, _testBoardId);

    var result = task.MoveTo(TaskStatus.InProgress, 5);

    result.IsSuccess.ShouldBeTrue();
    task.Status.ShouldBe(TaskStatus.InProgress);
    task.Position.ShouldBe(5);
  }

  [Fact]
  public void RaisesTaskMovedEvent()
  {
    var task = new KanbanTask(_testTitle, _testBoardId);

    task.MoveTo(TaskStatus.InProgress, 0);

    var domainEvents = task.DomainEvents.ToList();
    domainEvents.ShouldContain(e => e is TaskMovedEvent);

    var movedEvent = domainEvents.OfType<TaskMovedEvent>().First();
    movedEvent.TaskId.ShouldBe(task.Id);
    movedEvent.FromStatus.ShouldBe(TaskStatus.ToDo);
    movedEvent.ToStatus.ShouldBe(TaskStatus.InProgress);
    movedEvent.BoardId.ShouldBe(_testBoardId);
  }

  [Fact]
  public void PreventsMoveFromToDoToDone()
  {
    var task = new KanbanTask(_testTitle, _testBoardId);

    var result = task.MoveTo(TaskStatus.Done, 0);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(ResultStatus.Invalid);
    task.Status.ShouldBe(TaskStatus.ToDo); // Should remain unchanged
  }

  [Fact]
  public void AllowsMoveFromInProgressToDone()
  {
    var task = new KanbanTask(_testTitle, _testBoardId);
    task.MoveTo(TaskStatus.InProgress, 0);

    var result = task.MoveTo(TaskStatus.Done, 0);

    result.IsSuccess.ShouldBeTrue();
    task.Status.ShouldBe(TaskStatus.Done);
  }

  [Fact]
  public void AllowsMoveBackFromInProgressToToDo()
  {
    var task = new KanbanTask(_testTitle, _testBoardId);
    task.MoveTo(TaskStatus.InProgress, 0);

    var result = task.MoveTo(TaskStatus.ToDo, 0);

    result.IsSuccess.ShouldBeTrue();
    task.Status.ShouldBe(TaskStatus.ToDo);
  }

  [Fact]
  public void ThrowsExceptionForNegativePosition()
  {
    var task = new KanbanTask(_testTitle, _testBoardId);

    Should.Throw<ArgumentException>(() => task.MoveTo(TaskStatus.InProgress, -1));
  }
}
