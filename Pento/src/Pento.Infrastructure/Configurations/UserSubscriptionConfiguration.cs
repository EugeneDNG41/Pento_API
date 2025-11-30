using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Subscriptions;
using Pento.Domain.Users;
using Pento.Domain.UserSubscriptions;

namespace Pento.Infrastructure.Configurations;

internal sealed class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("user_subscriptions");
        builder.HasKey(us => us.Id);
        builder.Property(us => us.CancellationReason).HasMaxLength(500).IsRequired(false);
        builder.Property(us => us.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.HasOne<User>().WithMany().HasForeignKey(us => us.UserId);
        builder.HasOne<Subscription>().WithMany().HasForeignKey(us => us.SubscriptionId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
