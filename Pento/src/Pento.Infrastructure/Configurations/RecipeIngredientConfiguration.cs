using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodReferences;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Configurations;

internal sealed class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipe_ingredients");

        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Quantity)
               .IsRequired();

        builder.Property(ri => ri.Notes)
               .HasMaxLength(255);

        builder.Property(ri => ri.CreatedOnUtc)
               .IsRequired();

        builder.Property(ri => ri.UpdatedOnUtc)
               .IsRequired();

        builder.HasOne<Recipe>()
               .WithMany()
               .HasForeignKey(ri => ri.RecipeId);

        builder.HasOne<FoodReference>()
               .WithMany()
               .HasForeignKey(ri => ri.FoodRefId);
        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(ri => ri.UnitId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
