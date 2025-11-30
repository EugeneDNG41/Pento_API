
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Milestones;
using Pento.Domain.UserMilestones;

namespace Pento.Infrastructure.Configurations;

internal sealed class UserMilestoneConfiguration : IEntityTypeConfiguration<UserMilestone>
{
    public void Configure(EntityTypeBuilder<UserMilestone> builder)
    {
        builder.ToTable("user_milestones");
        builder.HasKey(um => um.Id);
        builder.HasOne<Milestone>()
            .WithMany()
            .HasForeignKey(um => um.MilestoneId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(um => !um.IsDeleted);
    }
}
