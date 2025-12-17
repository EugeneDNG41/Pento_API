using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Trades;
using Pento.Domain.Trades.Reports;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class TradeReportConfiguration : IEntityTypeConfiguration<TradeReport>
{
    public void Configure(EntityTypeBuilder<TradeReport> builder)
    {
        builder.ToTable("trade_reports");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TradeSessionId)
               .IsRequired();

        builder.Property(x => x.ReporterUserId)
               .IsRequired();

        builder.Property(x => x.ReportedUserId)
               .IsRequired();

        builder.Property(x => x.Reason)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.Severity)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(1000);

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.CreatedOn)
               .IsRequired();

        builder.Property(x => x.UpdatedOn);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.ReporterUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.ReportedUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
