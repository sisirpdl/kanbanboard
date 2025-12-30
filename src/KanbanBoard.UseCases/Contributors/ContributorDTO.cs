using KanbanBoard.Core.ContributorAggregate;

namespace KanbanBoard.UseCases.Contributors;
public record ContributorDto(ContributorId Id, ContributorName Name, PhoneNumber PhoneNumber);
