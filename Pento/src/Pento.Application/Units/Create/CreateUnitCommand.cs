using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Units;

namespace Pento.Application.Units.Create;
public sealed record CreateUnitCommand(
    string Name,
    string Abbreviation,
    decimal ToBaseFactor,
    UnitType Type
) : ICommand<Guid>;
