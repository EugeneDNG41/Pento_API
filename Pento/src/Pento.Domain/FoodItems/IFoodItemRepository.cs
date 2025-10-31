using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.FoodItems.Events;

namespace Pento.Domain.FoodItems;

public interface IFoodItemRepository
{
    Task<FoodItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task StartStreamAsync(FoodItemCreated e, CancellationToken cancellationToken = default);
}
