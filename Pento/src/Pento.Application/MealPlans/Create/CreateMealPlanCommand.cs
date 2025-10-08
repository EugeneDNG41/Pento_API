using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Create;
public sealed record CreateMealPlanCommand(
    Guid HouseholdId,
    string Name,
    Guid CreatedBy,
    DateOnly StartDate,
    DateOnly EndDate
) : ICommand<Guid>;
