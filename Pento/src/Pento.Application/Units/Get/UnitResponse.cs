using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Units.Get;
public sealed record UnitResponse(
    Guid Id,
    string Name,
    string Abbreviation,
    decimal ToBaseFactor,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
