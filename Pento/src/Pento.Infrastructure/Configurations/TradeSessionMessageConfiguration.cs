using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

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

