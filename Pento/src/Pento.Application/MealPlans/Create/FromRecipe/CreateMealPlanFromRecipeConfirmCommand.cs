using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Create.FromRecipe;
public sealed record CreateMealPlanFromRecipeConfirmCommand(
    Guid RecipeId,
    MealType MealType,
    DateOnly ScheduledDate,
    int Servings
) : ICommand<MealPlanAutoReserveResult>;
