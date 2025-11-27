using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Subscriptions;

namespace Pento.Infrastructure.Configurations;

internal sealed class SubscriptionFeatureConfiguration : IEntityTypeConfiguration<SubscriptionFeature>
{
    public void Configure(EntityTypeBuilder<SubscriptionFeature> builder)
    {
        builder.ToTable("subscription_features");
        builder.HasKey(sf => sf.Id);
        builder.Property(f => f.Quota).IsRequired(false);
        builder.Property(sf => sf.ResetPeriod).HasConversion<string>().HasMaxLength(10).IsRequired(false);
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);
        builder.HasOne<Subscription>().WithMany().HasForeignKey(sp => sp.SubscriptionId);
        builder.HasOne<Feature>().WithMany().HasForeignKey(sp => sp.FeatureCode);
    }
}
