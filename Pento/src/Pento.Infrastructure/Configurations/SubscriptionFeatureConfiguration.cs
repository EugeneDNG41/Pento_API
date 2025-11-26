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
        builder.OwnsOne(sf => sf.Entitlement, limitBuilder => limitBuilder.Property(limit => limit.ResetPer)
                .HasConversion<string>().HasMaxLength(50).IsRequired(false));
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);
        builder.HasOne<Subscription>().WithMany().HasForeignKey(sp => sp.SubscriptionId);
        builder.HasOne<Feature>().WithMany().HasForeignKey(sp => sp.FeatureName);
    }
}
