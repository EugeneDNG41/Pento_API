using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Reserve.Fullfill;
public sealed record FulfillMealPlanCommand(
    Guid MealPlanId
) : ICommand<Guid>;
