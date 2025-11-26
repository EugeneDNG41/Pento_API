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
        builder.OwnsOne(sp => sp.Price, moneyBuilder => moneyBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code)).HasMaxLength(3));
        builder.OwnsOne(sp => sp.Duration, durationBuilder => durationBuilder.Property(duration => duration.Unit)
                .HasConversion<string>().HasMaxLength(10));
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);
        builder.HasOne<Subscription>().WithMany().HasForeignKey(sp => sp.SubscriptionId);
    }
}
