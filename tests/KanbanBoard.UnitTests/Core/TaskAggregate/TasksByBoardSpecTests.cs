using KanbanBoard.Core.TaskAggregate;
using KanbanBoard.Core.TaskAggregate.Specifications;

namespace KanbanBoard.UnitTests.Core.TaskAggregate;

public class TasksByBoardSpecTests
{
  private readonly Guid _boardId = Guid.NewGuid();
  private readonly List<KanbanTask> _tasks;

  public TasksByBoardSpecTests()
  {
    var otherBoardId = Guid.NewGuid();

    _tasks = new List<KanbanTask>
    {
      new KanbanTask("Task 1", _boardId) { },
      new KanbanTask("Task 2", _boardId) { },
      new KanbanTask("Task 3", otherBoardId) { }, // Different board
      new KanbanTask("Task 4", _boardId) { }
    };

    // Move some tasks to different statuses
    _tasks[1].MoveTo(TaskStatus.InProgress, 0);
    _tasks[3].MoveTo(TaskStatus.Done, 0);
  }

  [Fact]
  public void FiltersTasksByBoardId()
  {
    var spec = new TasksByBoardSpec(_boardId);

    var result = spec.Evaluate(_tasks).ToList();

    result.Count.ShouldBe(3);
    result.ShouldAllBe(t => t.BoardId == _boardId);
  }

  [Fact]
  public void FiltersTasksByBoardIdAndStatus()
  {
    var spec = new TasksByBoardSpec(_boardId, TaskStatus.InProgress);

    var result = spec.Evaluate(_tasks).ToList();

    result.Count.ShouldBe(1);
    result[0].Title.ShouldBe("Task 2");
    result[0].Status.ShouldBe(TaskStatus.InProgress);
  }

  [Fact]
  public void OrdersTasksByStatusThenPosition()
  {
    // All tasks should be ordered by DisplayOrder (0, 1, 2) then by Position
    var spec = new TasksByBoardSpec(_boardId);

    var result = spec.Evaluate(_tasks).ToList();

    result[0].Status.ShouldBe(TaskStatus.ToDo);    // DisplayOrder 0
    result[1].Status.ShouldBe(TaskStatus.InProgress); // DisplayOrder 1
    result[2].Status.ShouldBe(TaskStatus.Done);    // DisplayOrder 2
  }
}
