using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.PossibleUnits.Get;
public sealed record GetPossibleUnitsQuery(Guid FoodRefId)
    : IQuery<IReadOnlyList<PossibleUnitResponse>>;
