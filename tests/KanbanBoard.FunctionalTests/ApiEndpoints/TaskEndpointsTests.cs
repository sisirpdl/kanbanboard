using System.Net;
using System.Net.Http.Json;
using KanbanBoard.UseCases.Tasks;
using KanbanBoard.Web.Tasks;

namespace KanbanBoard.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class TaskEndpointsTests(CustomWebApplicationFactory<Program> factory)
  : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();
  private readonly Guid _boardId = Guid.NewGuid();

  [Fact]
  public async Task CreateTask_ReturnsCreated()
  {
    // Arrange
    var request = new CreateTaskRequest
    {
      BoardId = _boardId,
      Title = "Test Task",
      Description = "Test Description"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/Tasks", request);

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.Created);
    var result = await response.Content.ReadFromJsonAsync<CreateTaskResponse>();
    result.ShouldNotBeNull();
    result!.Id.ShouldNotBe(Guid.Empty);
    result.Title.ShouldBe("Test Task");
  }

  [Fact]
  public async Task CreateTask_WithoutTitle_ReturnsBadRequest()
  {
    // Arrange
    var request = new CreateTaskRequest
    {
      BoardId = _boardId,
      Title = "" // Empty title
    };

    // Act
    var response = await _client.PostAsJsonAsync("/Tasks", request);

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task GetTaskById_ReturnsTask()
  {
    // Arrange - Create a task first
    var createRequest = new CreateTaskRequest
    {
      BoardId = _boardId,
      Title = "Test Task"
    };
    var createResponse = await _client.PostAsJsonAsync("/Tasks", createRequest);
    var created = await createResponse.Content.ReadFromJsonAsync<CreateTaskResponse>();

    // Act
    var response = await _client.GetAsync($"/Tasks/{created!.Id}");

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    var task = await response.Content.ReadFromJsonAsync<TaskDto>();
    task.ShouldNotBeNull();
    task!.Title.ShouldBe("Test Task");
  }

  [Fact]
  public async Task GetTaskById_NonExistent_ReturnsNotFound()
  {
    // Act
    var response = await _client.GetAsync($"/Tasks/{Guid.NewGuid()}");

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task MoveTask_ValidTransition_ReturnsOk()
  {
    // Arrange - Create a task
    var createRequest = new CreateTaskRequest
    {
      BoardId = _boardId,
      Title = "Task to Move"
    };
    var createResponse = await _client.PostAsJsonAsync("/Tasks", createRequest);
    var created = await createResponse.Content.ReadFromJsonAsync<CreateTaskResponse>();

    var moveRequest = new MoveTaskRequest
    {
      TaskId = created!.Id,
      NewStatus = "InProgress",
      NewPosition = 0
    };

    // Act
    var response = await _client.PatchAsJsonAsync($"/Tasks/{created.Id}/move", moveRequest);

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
  }

  [Fact]
  public async Task MoveTask_InvalidTransition_ReturnsBadRequest()
  {
    // Arrange - Create a task
    var createRequest = new CreateTaskRequest
    {
      BoardId = _boardId,
      Title = "Task to Move"
    };
    var createResponse = await _client.PostAsJsonAsync("/Tasks", createRequest);
    var created = await createResponse.Content.ReadFromJsonAsync<CreateTaskResponse>();

    var moveRequest = new MoveTaskRequest
    {
      TaskId = created!.Id,
      NewStatus = "Done", // Can't skip from ToDo to Done
      NewPosition = 0
    };

    // Act
    var response = await _client.PatchAsJsonAsync($"/Tasks/{created.Id}/move", moveRequest);

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task UpdateTask_ReturnsOk()
  {
    // Arrange - Create a task
    var createRequest = new CreateTaskRequest
    {
      BoardId = _boardId,
      Title = "Original Title"
    };
    var createResponse = await _client.PostAsJsonAsync("/Tasks", createRequest);
    var created = await createResponse.Content.ReadFromJsonAsync<CreateTaskResponse>();

    var updateRequest = new UpdateTaskRequest
    {
      TaskId = created!.Id,
      Title = "Updated Title",
      Description = "Updated Description"
    };

    // Act
    var response = await _client.PutAsJsonAsync($"/Tasks/{created.Id}", updateRequest);

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
  }

  [Fact]
  public async Task DeleteTask_ReturnsNoContent()
  {
    // Arrange - Create a task
    var createRequest = new CreateTaskRequest
    {
      BoardId = _boardId,
      Title = "Task to Delete"
    };
    var createResponse = await _client.PostAsJsonAsync("/Tasks", createRequest);
    var created = await createResponse.Content.ReadFromJsonAsync<CreateTaskResponse>();

    // Act
    var response = await _client.DeleteAsync($"/Tasks/{created!.Id}");

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

    // Verify deletion
    var getResponse = await _client.GetAsync($"/Tasks/{created.Id}");
    getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task GetTasksByBoard_ReturnsTasksList()
  {
    // Arrange - Create multiple tasks
    var task1 = new CreateTaskRequest { BoardId = _boardId, Title = "Task 1" };
    var task2 = new CreateTaskRequest { BoardId = _boardId, Title = "Task 2" };

    await _client.PostAsJsonAsync("/Tasks", task1);
    await _client.PostAsJsonAsync("/Tasks", task2);

    // Act
    var response = await _client.GetAsync($"/Boards/{_boardId}/tasks");

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
    tasks.ShouldNotBeNull();
    tasks!.Count.ShouldBeGreaterThanOrEqualTo(2);
  }

  [Fact]
  public async Task GetTasksByBoard_WithStatusFilter_ReturnsFilteredTasks()
  {
    // Arrange - Create tasks and move one to InProgress
    var createRequest = new CreateTaskRequest { BoardId = _boardId, Title = "Task" };
    var createResponse = await _client.PostAsJsonAsync("/Tasks", createRequest);
    var created = await createResponse.Content.ReadFromJsonAsync<CreateTaskResponse>();

    var moveRequest = new MoveTaskRequest
    {
      TaskId = created!.Id,
      NewStatus = "InProgress",
      NewPosition = 0
    };
    await _client.PatchAsJsonAsync($"/Tasks/{created.Id}/move", moveRequest);

    // Act
    var response = await _client.GetAsync($"/Boards/{_boardId}/tasks?filterStatus=InProgress");

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
    tasks.ShouldNotBeNull();
    tasks!.ShouldAllBe(t => t.Status == "InProgress");
  }
}
