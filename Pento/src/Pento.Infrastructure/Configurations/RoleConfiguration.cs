using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Name);

        builder.Property(r => r.Name).HasMaxLength(50);
        builder.Property(r => r.Type).HasConversion<string>().HasMaxLength(50);

        builder
            .HasMany<User>()
            .WithMany(u => u.Roles)
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("user_roles");
                joinBuilder.Property("RolesName").HasColumnName("role_name");
            });

        builder.HasData(
            Role.Administrator,
            Role.HouseholdHead,
            Role.PowerMember,
            Role.GroceryShopper,
            Role.MealPlanner,
            Role.PantryManager,
            Role.ErrandRunner);
    }
}
