using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Get;
public sealed record GetMealPlansByHouseholdIdQuery(Guid HouseholdId)
    : IQuery<IReadOnlyList<MealPlanResponse>>;
