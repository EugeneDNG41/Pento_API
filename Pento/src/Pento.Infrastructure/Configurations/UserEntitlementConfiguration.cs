using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Features;
using Pento.Domain.UserEntitlements;
using Pento.Domain.Users;
using Pento.Domain.UserSubscriptions;

namespace Pento.Infrastructure.Configurations;

internal sealed class UserEntitlementConfiguration : IEntityTypeConfiguration<UserEntitlement>
{
    public void Configure(EntityTypeBuilder<UserEntitlement> builder)
    {
        builder.ToTable("user_entitlements");
        builder.HasKey(ue => ue.Id);
        builder.Property(f => f.Quota).IsRequired(false);
        builder.Property(sf => sf.ResetPeriod).HasConversion<string>().HasMaxLength(10).IsRequired(false);
        builder.HasOne<User>().WithMany().HasForeignKey(ue => ue.UserId);
        builder.HasOne<Feature>().WithMany().HasForeignKey(ue => ue.FeatureCode);
        builder.HasOne<UserSubscription>().WithMany().HasForeignKey(ue => ue.UserSubscriptionId);
        builder.Property<uint>("Version").IsRowVersion();
    }
}
