using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Create;
public sealed record CreateDetailedRecipeCommand(
    string Title,
    string Description,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    string? Notes,
    int? Servings,
    DifficultyLevel? DifficultyLevel,
    string? Image,
    bool IsPublic,
    List<RecipeIngredientRequest> Ingredients,
    List<RecipeDirectionRequest> Directions
) : ICommand<Guid>;

public sealed record RecipeIngredientRequest(
    Guid FoodRefId,
    decimal Quantity,
    Guid UnitId,
    string? Notes
);

public sealed record RecipeDirectionRequest(
    int StepNumber,
    string Description,
    string? Image
);
