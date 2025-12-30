using KanbanBoard.Core.TaskAggregate;

namespace KanbanBoard.Infrastructure.Data.Config;

public class KanbanTaskConfiguration : IEntityTypeConfiguration<KanbanTask>
{
  public void Configure(EntityTypeBuilder<KanbanTask> builder)
  {
    builder.ToTable("Tasks");

    builder.Property(t => t.Id)
      .IsRequired();

    builder.Property(t => t.Title)
      .HasMaxLength(200)
      .IsRequired();

    builder.Property(t => t.Description)
      .HasMaxLength(2000);

    builder.Property(t => t.Status)
      .HasConversion(
        v => v.Value,
        v => Core.TaskAggregate.TaskStatus.FromValue(v))
      .IsRequired();

    builder.Property(t => t.BoardId)
      .IsRequired();

    builder.Property(t => t.Position)
      .IsRequired();

    // Index for common query patterns
    builder.HasIndex(t => new { t.BoardId, t.Status, t.Position });
  }
}
