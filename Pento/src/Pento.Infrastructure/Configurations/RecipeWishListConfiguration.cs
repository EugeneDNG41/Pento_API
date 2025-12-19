using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Households;
using Pento.Domain.Recipes;
using Pento.Domain.RecipeWishLists;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class RecipeWishListConfiguration : IEntityTypeConfiguration<RecipeWishList>
{
    public void Configure(EntityTypeBuilder<RecipeWishList> builder)
    {
        builder.ToTable("recipe_wishlists");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.HouseholdId)
            .IsRequired();

        builder.Property(x => x.RecipeId)
            .IsRequired();

        builder.Property(x => x.AddedOnUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.HouseholdId, x.RecipeId })
            .IsUnique();
        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(x => x.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
