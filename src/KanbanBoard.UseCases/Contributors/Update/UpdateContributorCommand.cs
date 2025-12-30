using KanbanBoard.Core.ContributorAggregate;

namespace KanbanBoard.UseCases.Contributors.Update;

public record UpdateContributorCommand(ContributorId ContributorId, ContributorName NewName) : ICommand<Result<ContributorDto>>;
