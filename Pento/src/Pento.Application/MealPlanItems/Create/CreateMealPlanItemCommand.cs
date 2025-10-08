using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlanItems.Create;
public sealed record CreateMealPlanItemCommand(
    Guid MealPlanId,
    Guid RecipeId,
    string MealType,                 
    List<DateTime> Schedule,
    int Servings,
    string? Notes
) : ICommand<Guid>;
