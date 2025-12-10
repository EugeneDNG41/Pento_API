using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodItems;
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
internal sealed class TradeRequestConfiguration : IEntityTypeConfiguration<TradeRequest>
{
    public void Configure(EntityTypeBuilder<TradeRequest> builder)
    {
        builder.ToTable("trade_requests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.TradeOfferId).IsRequired();

        builder.Property(x => x.Status)
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
        builder.HasOne<TradeOffer>()
               .WithMany()
               .HasForeignKey(x => x.TradeOfferId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
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
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.OfferUserId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.RequestUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
internal sealed class TradeSessionMessageConfiguration : IEntityTypeConfiguration<TradeSessionMessage>
{
    public void Configure(EntityTypeBuilder<TradeSessionMessage> builder)
    {
        builder.ToTable("trade_session_messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TradeSessionId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.MessageText)
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(x => x.SentOn).IsRequired();

        builder.HasOne<TradeSession>()
               .WithMany()
               .HasForeignKey(x => x.TradeSessionId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
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
               .HasValue<TradeItemRequest>(TradeItemFrom.Request)
               .HasValue<TradeItemSession>(TradeItemFrom.Session);
        builder.HasOne<FoodItem>()
               .WithMany()
               .HasForeignKey(x => x.FoodItemId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => true);
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
internal sealed class TradeItemSessionConfiguration : IEntityTypeConfiguration<TradeItemSession>
{
    public void Configure(EntityTypeBuilder<TradeItemSession> builder)
    {
        builder.Property(x => x.SessionId)
               .IsRequired();
        builder.Property(x => x.ItemFrom)
               .HasConversion<string>()
               .HasMaxLength(10)
               .IsRequired();
        builder.HasOne<TradeSession>()
               .WithMany()
               .HasForeignKey(x => x.SessionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

