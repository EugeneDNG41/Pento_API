using Pento.Domain.Abstractions;
using Pento.Domain.StorageItems.Events;

namespace Pento.Domain.StorageItems;

public static class StorageItemErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("StorageItems.IdentityNotFound", $"The storage item with the identifier {id} not found");
}
