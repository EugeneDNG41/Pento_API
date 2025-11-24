using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.MealPlanRecipe;
using Pento.Domain.MealPlans;

namespace Pento.Infrastructure.Configurations;

public sealed class MealPlanRecipeConfiguration : IEntityTypeConfiguration<MealPlanRecipe>
{
    public void Configure(EntityTypeBuilder<MealPlanRecipe> builder)
    {
        builder.ToTable("meal_plan_recipes");

        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.MealPlan)
            .WithMany()
            .HasForeignKey(x => x.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);   

        builder.Property(x => x.MealPlanId)
            .IsRequired();   

        builder.Property(x => x.RecipeId)
            .IsRequired();

        builder.HasIndex(x => new { x.MealPlanId, x.RecipeId })
            .IsUnique();

        builder.HasQueryFilter(x => !x.MealPlan.IsDeleted);
    }
}
