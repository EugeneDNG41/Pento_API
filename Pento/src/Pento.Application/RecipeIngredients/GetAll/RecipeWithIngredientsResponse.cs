using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.RecipeIngredients.GetAll;
public sealed class RecipeWithIngredientsResponse
{
    public Guid RecipeId { get; init; }
    public string RecipeTitle { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<RecipeIngredientItemResponse> Ingredients { get; init; } = [];
}

public sealed class RecipeIngredientItemResponse
{
    public Guid IngredientId { get; init; }
    public Guid FoodRefId { get; init; }
    public string FoodRefName { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public Guid UnitId { get; init; }
    public string UnitName { get; init; } = string.Empty;
    public string? Notes { get; init; }
}
