using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class HouseholdConfiguration : IEntityTypeConfiguration<Household>
{
    public void Configure(EntityTypeBuilder<Household> builder)
    {
        builder.ToTable("households");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Name).HasMaxLength(200);

        builder.Property(h => h.InviteCode).HasMaxLength(15).IsRequired(false);

        builder.Property(h => h.InviteCodeExpirationUtc).IsRequired(false);

        builder.HasMany<User>().WithOne().HasForeignKey(u => u.HouseholdId).IsRequired(false);
    }
}
