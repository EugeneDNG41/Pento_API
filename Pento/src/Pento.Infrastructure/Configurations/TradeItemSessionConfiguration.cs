using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Configurations;

internal sealed class TradeItemSessionConfiguration : IEntityTypeConfiguration<TradeSessionItem>
{
    public void Configure(EntityTypeBuilder<TradeSessionItem> builder)
    {
        builder.Property(x => x.SessionId)
               .IsRequired();
        builder.Property(x => x.FoodItemId).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.UnitId).IsRequired();

        builder.Property(x => x.From)
               .HasConversion<string>()
               .HasMaxLength(10)
               .IsRequired();
        builder.HasOne<TradeSession>()
               .WithMany()
               .HasForeignKey(x => x.SessionId)
               .OnDelete(DeleteBehavior.Cascade);
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

