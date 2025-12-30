using KanbanBoard.Core.ContributorAggregate;

namespace KanbanBoard.UseCases.Contributors.Delete;

public record DeleteContributorCommand(ContributorId ContributorId) : ICommand<Result>;
