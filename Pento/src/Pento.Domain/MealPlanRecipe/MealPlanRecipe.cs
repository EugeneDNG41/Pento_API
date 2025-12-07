using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.Domain.MealPlanRecipe;

public sealed class MealPlanRecipe : Entity
{
    public Guid MealPlanId { get; private set; }
    public Guid RecipeId { get; private set; }

    private MealPlanRecipe() { }

    public MealPlanRecipe(Guid mealPlanId, Guid recipeId)
    {
        Id = Guid.CreateVersion7();
        MealPlanId = mealPlanId;
        RecipeId = recipeId;
    }

    public static MealPlanRecipe Create(Guid mealPlanId, Guid recipeId)
        => new MealPlanRecipe(mealPlanId, recipeId);
    public MealPlan MealPlan { get; private set; } = null!;

}

