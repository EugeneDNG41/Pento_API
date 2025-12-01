using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.RecipeWishLists;

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
        builder.HasQueryFilter(x => !x.IsDeleted);

    }
}
