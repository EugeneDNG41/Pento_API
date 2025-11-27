using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;

namespace Pento.Infrastructure.Configurations;

internal sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("subscription_plans");
        builder.HasKey(sp => sp.Id);
        builder.Property(sp => sp.Amount).IsRequired();
        builder.Property(sp => sp.Currency).HasConversion<string>().HasMaxLength(3).IsRequired();
        builder.Property(sp => sp.DurationInDays).IsRequired(false);
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);
        builder.HasOne<Subscription>().WithMany().HasForeignKey(sp => sp.SubscriptionId);
    }
}
