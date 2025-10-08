using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.MealPlanItems.Get;
public sealed record MealPlanItemResponse(
     Guid Id,
     Guid MealPlanId,
     Guid RecipeId,
     DateOnly Date,
     string MealType,
     int Servings,
     DateTime CreatedOnUtc,
     DateTime UpdatedOnUtc
 );
