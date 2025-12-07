using Pento.Application.Abstractions.Messaging;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Update;

public sealed record UpdateMealPlanCommand(
    Guid Id,
    MealType MealType,
    DateOnly ScheduledDate,
    int Servings,
    string? Notes
) : ICommand;
