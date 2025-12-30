using KanbanBoard.Core.TaskAggregate;

namespace KanbanBoard.UnitTests.Core.TaskAggregate;

public class TaskStatusTests
{
  [Fact]
  public void ToDoHasCorrectProperties()
  {
    TaskStatus.ToDo.Name.ShouldBe("ToDo");
    TaskStatus.ToDo.Value.ShouldBe(1);
    TaskStatus.ToDo.DisplayOrder.ShouldBe(0);
  }

  [Fact]
  public void InProgressHasCorrectProperties()
  {
    TaskStatus.InProgress.Name.ShouldBe("InProgress");
    TaskStatus.InProgress.Value.ShouldBe(2);
    TaskStatus.InProgress.DisplayOrder.ShouldBe(1);
  }

  [Fact]
  public void DoneHasCorrectProperties()
  {
    TaskStatus.Done.Name.ShouldBe("Done");
    TaskStatus.Done.Value.ShouldBe(3);
    TaskStatus.Done.DisplayOrder.ShouldBe(2);
  }

  [Fact]
  public void CanTransitionFromToDoToInProgress()
  {
    var canTransition = TaskStatus.ToDo.CanTransitionTo(TaskStatus.InProgress);

    canTransition.ShouldBeTrue();
  }

  [Fact]
  public void CannotTransitionFromToDoToDone()
  {
    var canTransition = TaskStatus.ToDo.CanTransitionTo(TaskStatus.Done);

    canTransition.ShouldBeFalse();
  }

  [Fact]
  public void CanTransitionFromInProgressToDone()
  {
    var canTransition = TaskStatus.InProgress.CanTransitionTo(TaskStatus.Done);

    canTransition.ShouldBeTrue();
  }

  [Fact]
  public void CanTransitionFromInProgressBackToToDo()
  {
    var canTransition = TaskStatus.InProgress.CanTransitionTo(TaskStatus.ToDo);

    canTransition.ShouldBeTrue();
  }

  [Fact]
  public void CanParseFromName()
  {
    var status = TaskStatus.FromName("InProgress");

    status.ShouldBe(TaskStatus.InProgress);
  }

  [Fact]
  public void CanParseFromValue()
  {
    var status = TaskStatus.FromValue(2);

    status.ShouldBe(TaskStatus.InProgress);
  }
}
