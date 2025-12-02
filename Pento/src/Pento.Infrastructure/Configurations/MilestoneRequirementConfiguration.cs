
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Activities;
using Pento.Domain.MilestoneRequirements;
using Pento.Domain.Milestones;

namespace Pento.Infrastructure.Configurations;

internal sealed class MilestoneRequirementConfiguration : IEntityTypeConfiguration<MilestoneRequirement>
{
    public void Configure(EntityTypeBuilder<MilestoneRequirement> builder)
    {
        builder.ToTable("milestone_requirements");
        builder.HasKey(mr => mr.Id);
        builder.HasOne<Milestone>()
            .WithMany()
            .HasForeignKey(mr => mr.MilestoneId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(mr => mr.ActivityCode)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
