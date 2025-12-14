
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Features;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserEntitlements;

namespace Pento.Infrastructure.Configurations;

internal sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("features");
        builder.HasKey(f => f.Code);
        builder.Property(f => f.Code).HasConversion<string>().HasMaxLength(50);
        builder.Property(f => f.Name).HasMaxLength(100);
        builder.Property(f => f.Description).HasMaxLength(500);
        builder.Property(f => f.DefaultQuota).IsRequired(false);
        builder.Property(sf => sf.DefaultResetPeriod).HasConversion<string>().HasMaxLength(10).IsRequired(false);
        builder.HasMany<SubscriptionFeature>()
            .WithOne()
            .HasForeignKey(sf => sf.FeatureCode);
        builder.HasMany<UserEntitlement>()
            .WithOne()
            .HasForeignKey(ue => ue.FeatureCode);
        builder.HasData(
            Feature.OCR,
            Feature.ImageRecognition,
            Feature.AIChef,
            Feature.GroceryMap
        );
    }
}
