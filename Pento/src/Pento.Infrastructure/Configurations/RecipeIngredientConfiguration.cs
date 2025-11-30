using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.RecipeIngredients;

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

        builder.HasOne<Domain.Recipes.Recipe>()
               .WithMany()
               .HasForeignKey(ri => ri.RecipeId);

        builder.HasOne<Domain.FoodReferences.FoodReference>()
               .WithMany()
               .HasForeignKey(ri => ri.FoodRefId);
        builder.HasOne<Domain.Units.Unit>()
            .WithMany()
            .HasForeignKey(ri => ri.UnitId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
