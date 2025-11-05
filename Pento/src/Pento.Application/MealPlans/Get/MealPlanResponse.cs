using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.MealPlans.Get;
public sealed record MealPlanResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    DateOnly ScheduledDate,
    int Servings,
    string? Notes,
    Guid CreatedBy,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
