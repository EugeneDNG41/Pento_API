using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Payments;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.OrderCode).ValueGeneratedOnAdd();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Currency).HasMaxLength(10);
        builder.Property(p => p.CancellationReason).HasMaxLength(500).IsRequired(false);
        builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
