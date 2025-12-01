
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Activities;

namespace Pento.Infrastructure.Configurations;

internal sealed class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("activities");
        builder.HasKey(a => a.Code);
        builder.Property(a => a.Code).HasMaxLength(50);
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(10);
        builder.Property(a => a.Name).HasMaxLength(100);
        builder.Property(a => a.Description).HasMaxLength(500);
        builder.HasData(
            Activity.CreateStorage,
            Activity.ConsumeFoodItem,
            Activity.CreateHousehold,
            Activity.StorageQuantity,
            Activity.StorageTypePantry,
            Activity.StorageTypeRefrigerator,
            Activity.StorageTypeFreezer,
            Activity.CompartmentQuantity,
            Activity.FoodItemQuantity);
    }
}
