using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Messaging;

namespace Pento.Modules.Pantry.Application.FoodItems.Get;

public sealed record GetFoodItemQuery(Guid FoodItemId) : IQuery<FoodItemResponse>;
