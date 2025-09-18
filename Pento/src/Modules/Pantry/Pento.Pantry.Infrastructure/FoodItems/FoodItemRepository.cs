using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Modules.Pantry.Domain.FoodItems;
using Pento.Modules.Pantry.Infrastructure.Database;

namespace Pento.Modules.Pantry.Infrastructure.FoodItems;

internal sealed class FoodItemRepository(/*PantryDbContext context*/) : IFoodItemRepository
{
}
