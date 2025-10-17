using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Units.Create;
public sealed record CreateUnitCommand(
    string Name,
    string Abbreviation,
    decimal ToBaseFactor
) : ICommand<Guid>;
