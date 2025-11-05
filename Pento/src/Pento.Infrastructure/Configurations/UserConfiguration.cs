using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName).HasMaxLength(200);

        builder.Property(u => u.LastName).HasMaxLength(200);

        builder.Property(u => u.Email).HasMaxLength(300);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasIndex(u => u.IdentityId).IsUnique();

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(u => u.HouseholdId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasQueryFilter(c => !c.IsDeleted);

    }
}
