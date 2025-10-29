using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten.Events.Aggregation;
using Pento.Domain.StorageItems;

namespace Pento.Application.StorageItems.Projections;

public sealed class InventoryProjection() : SingleStreamProjection<StorageItem, Guid>
{

}
