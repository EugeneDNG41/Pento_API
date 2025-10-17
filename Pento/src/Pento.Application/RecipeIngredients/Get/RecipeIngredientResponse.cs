using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.RecipeIngredients.Get;

public sealed record RecipeIngredientResponse(
    Guid Id,
    Guid RecipeId,
    Guid FoodRefId,
    decimal Quantity,
    Guid UnitId,
    string? Notes,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
