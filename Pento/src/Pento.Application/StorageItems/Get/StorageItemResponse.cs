using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.StorageItems.Get;

public sealed record StorageItemResponse(
    Guid Id,
    Guid FoodRefId,
    Guid CompartmentId,
    string? CustomName,
    decimal Quantity,
    Guid UnitId,
    DateTime ExpirationDateUtc,
    string? Notes);
