using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlanItems.Get;
public sealed record GetMealPlanItemQuery(Guid MealPlanItemId) : IQuery<MealPlanItemResponse>
{
}
