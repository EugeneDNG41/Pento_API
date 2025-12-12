using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Configurations;

internal sealed class TradeItemConfiguration : IEntityTypeConfiguration<TradeItem>
{
    public void Configure(EntityTypeBuilder<TradeItem> builder)
    {
        builder.ToTable("trade_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FoodItemId).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.UnitId).IsRequired();

        builder.Property(x => x.From)
               .HasConversion<string>()
               .HasMaxLength(10)
               .IsRequired();

        builder.UseTphMappingStrategy()
               .HasDiscriminator(x => x.From)
               .HasValue<TradeItemOffer>(TradeItemFrom.Offer)
               .HasValue<TradeItemRequest>(TradeItemFrom.Request);
        builder.HasOne<FoodItem>()
               .WithMany()
               .HasForeignKey(x => x.FoodItemId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<FoodItem>()
               .WithMany()
               .HasForeignKey(x => x.FoodItemId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Unit>()
               .WithMany()
               .HasForeignKey(x => x.UnitId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
internal sealed class TradeItemOfferConfiguration : IEntityTypeConfiguration<TradeItemOffer>
{
    public void Configure(EntityTypeBuilder<TradeItemOffer> builder)
    {
        builder.Property(x => x.OfferId)
               .IsRequired();

        builder.HasOne<TradeOffer>()
               .WithMany()
               .HasForeignKey(x => x.OfferId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class TradeItemRequestConfiguration : IEntityTypeConfiguration<TradeItemRequest>
{
    public void Configure(EntityTypeBuilder<TradeItemRequest> builder)
    {
        builder.Property(x => x.RequestId)
               .IsRequired();

        builder.HasOne<TradeRequest>()
               .WithMany()
               .HasForeignKey(x => x.RequestId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
