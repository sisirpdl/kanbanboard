using KanbanBoard.Core.ContributorAggregate;

namespace KanbanBoard.UseCases.Contributors.Get;

public record GetContributorQuery(ContributorId ContributorId) : IQuery<Result<ContributorDto>>;
