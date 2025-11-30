using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Payments;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.OrderCode).ValueGeneratedOnAdd();
        builder.Property(p => p.Description).HasMaxLength(100);
        builder.Property(p => p.ProviderDescription).HasMaxLength(40).IsRequired(false);
        builder.Property(p => p.Currency).HasConversion<string>().HasMaxLength(3);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.CancellationReason).HasMaxLength(500).IsRequired(false);
        builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId);
        builder.HasOne<SubscriptionPlan>().WithMany().HasForeignKey(p => p.SubscriptionPlanId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
