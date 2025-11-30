
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Milestones;

namespace Pento.Infrastructure.Configurations;

internal sealed class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
{
    public void Configure(EntityTypeBuilder<Milestone> builder)
    {
        builder.ToTable("milestones");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).HasMaxLength(100);
        builder.Property(m => m.Description).HasMaxLength(500);
        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
