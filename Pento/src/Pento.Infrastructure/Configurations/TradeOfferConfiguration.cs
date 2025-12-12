using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class TradeOfferConfiguration : IEntityTypeConfiguration<TradeOffer>
{
    public void Configure(EntityTypeBuilder<TradeOffer> builder)
    {
        builder.ToTable("trade_offers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();

        builder.Property(x => x.PickupOption)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.CreatedOn).IsRequired();
        builder.Property(x => x.UpdatedOn);
        builder.HasOne<Household>()
               .WithMany()
               .HasForeignKey(x => x.HouseholdId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}


