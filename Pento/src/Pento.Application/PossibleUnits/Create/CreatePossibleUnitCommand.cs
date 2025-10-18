using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.PossibleUnits.Create;
public sealed record CreatePossibleUnitCommand(
    Guid UnitId,
    Guid FoodRefId,
    bool IsDefault
) : ICommand<Guid>;
