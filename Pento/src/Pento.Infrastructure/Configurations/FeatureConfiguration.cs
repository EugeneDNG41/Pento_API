
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Subscriptions;

namespace Pento.Infrastructure.Configurations;

internal sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("features");
        builder.HasKey(f => f.Name);
        builder.Property(f => f.Name).HasMaxLength(100);
        builder.HasData(
            Feature.OCR,
            Feature.ImageRecognition,
            Feature.AIChef,
            Feature.StorageSlot,
            Feature.MealPlanSlot);
    }
}
