using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Get;
public sealed record MealPlanResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    DateOnly ScheduledDate,
    string MealType,
    int Servings,
    string? Notes,
    Guid CreatedBy,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
