using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
