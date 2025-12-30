using KanbanBoard.Core.ContributorAggregate;
using Vogen;

namespace KanbanBoard.Infrastructure.Data.Config;

[EfCoreConverter<ContributorId>]
[EfCoreConverter<ContributorName>]
internal partial class VogenEfCoreConverters;
