using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.StorageItems.Events;

namespace Pento.Domain.StorageItems;

public interface IStorageItemRepository
{
    Task<StorageItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task StartStreamAsync(StorageItemCreated e, CancellationToken cancellationToken = default);
}
