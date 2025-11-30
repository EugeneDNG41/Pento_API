
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Activities;
using Pento.Domain.UserActivities;

namespace Pento.Infrastructure.Configurations;

internal sealed class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("user_activities");
        builder.HasKey(ua => ua.Id);
        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(ua => ua.ActivityCode)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
