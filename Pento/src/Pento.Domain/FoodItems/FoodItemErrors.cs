using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems.Events;

namespace Pento.Domain.FoodItems;

public static class FoodItemErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("FoodItems.IdentityNotFound", $"The storage item with the identifier {id} not found");
}
