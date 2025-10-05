using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.MealPlanItems;
public sealed class MealPlanItem : Entity
{
    public MealPlanItem(
        Guid id,
        Guid mealPlanId,
        Guid recipeId,
        MealType mealType,
        List<DateTime> schedule,
        int servings,
        string? notes,
        DateTime createdOnUtc)
        : base(id)
    {
        MealPlanId = mealPlanId;
        RecipeId = recipeId;
        Schedule = schedule;
        MealType = mealType;
        Servings = servings > 0 ? servings : 1;
        Notes = notes;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    private MealPlanItem()
    {
    }

    public Guid MealPlanId { get; private set; }

    public Guid RecipeId { get; private set; }

    public DateOnly MealDate { get; private set; }

    public MealType MealType { get; private set; }

    public int Servings { get; private set; }

    public string? Notes { get; private set; }

    public List<DateTime> Schedule { get; private set; } 
    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }

}
