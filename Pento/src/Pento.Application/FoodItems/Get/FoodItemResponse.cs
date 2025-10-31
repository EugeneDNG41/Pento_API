using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.FoodItems.Get;

public sealed record FoodItemResponse(
    Guid Id,
    Guid FoodRefId,
    Guid CompartmentId,
    string? CustomName,
    decimal Quantity,
    Guid UnitId,
    DateTime ExpirationDateUtc,
    string? Notes);
