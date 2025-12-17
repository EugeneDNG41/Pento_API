using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Trades;
using Pento.Domain.Trades.Reports;

namespace Pento.Infrastructure.Configurations;

internal sealed class TradeReportMediaConfiguration : IEntityTypeConfiguration<TradeReportMedia>
{
    public void Configure(EntityTypeBuilder<TradeReportMedia> builder)
    {
        builder.ToTable("trade_report_medias");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TradeReportId)
               .IsRequired();

        builder.Property(x => x.MediaType)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.MediaUri)
               .HasMaxLength(2048)
               .IsRequired();

        builder.Property(x => x.CreatedOn)
               .IsRequired();

        builder.HasOne<TradeReport>()
               .WithMany()
               .HasForeignKey(x => x.TradeReportId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
