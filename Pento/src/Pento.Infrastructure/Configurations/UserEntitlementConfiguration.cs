using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.UserEntitlements;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class UserEntitlementConfiguration : IEntityTypeConfiguration<UserEntitlement>
{
    public void Configure(EntityTypeBuilder<UserEntitlement> builder)
    {
        builder.ToTable("user_entitlements");
        builder.HasKey(ue => ue.Id);
        builder.Property(ue => ue.Feature).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.OwnsOne(ue => ue.Limit, limitBuilder => limitBuilder.Property(limit => limit.ResetPer)
                .HasConversion<string>().HasMaxLength(50).IsRequired(false));
        builder.HasOne<User>().WithMany().HasForeignKey(ue => ue.UserId);
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);
    }
}
