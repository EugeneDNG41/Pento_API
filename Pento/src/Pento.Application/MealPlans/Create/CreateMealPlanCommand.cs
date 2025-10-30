using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Create;
public sealed record CreateMealPlanCommand(
    Guid HouseholdId,
    Guid RecipeId,
    string Name,
    MealType MealType,
    DateOnly ScheduledDate,
    int Servings,
    string? Notes,
    Guid CreatedBy
) : ICommand<Guid>;
