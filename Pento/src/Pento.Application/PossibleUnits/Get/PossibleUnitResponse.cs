using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.PossibleUnits.Get;
public sealed record PossibleUnitResponse(
    Guid Id,
    Guid UnitId,
    Guid FoodRefId,
    bool IsDefault,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
