using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class TradeSessionConfiguration : IEntityTypeConfiguration<TradeSession>
{
    public void Configure(EntityTypeBuilder<TradeSession> builder)
    {
        builder.ToTable("trade_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TradeOfferId).IsRequired();
        builder.Property(x => x.TradeRequestId).IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.StartedOn).IsRequired();

        builder.HasOne<TradeOffer>()
               .WithMany()
               .HasForeignKey(x => x.TradeOfferId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<TradeRequest>()
               .WithMany()
               .HasForeignKey(x => x.TradeRequestId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Household>()
               .WithMany()
               .HasForeignKey(x => x.OfferHouseholdId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(x => x.RequestHouseholdId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.ConfirmedByOfferUser)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ConfirmedByRequestUser)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

