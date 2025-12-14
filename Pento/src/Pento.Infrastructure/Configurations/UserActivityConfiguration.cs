
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Activities;
using Pento.Domain.Households;
using Pento.Domain.UserActivities;
using Pento.Domain.Users;

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
        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(ua => ua.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
