using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Units;
public static class UnitErrors
{
    public static Error NotFound(Guid unitId) =>
        Error.NotFound("Units.IdentityNotFound", $"The unit with ID {unitId} was not found.");

    public static readonly Error InvalidName = Error.Problem(
        "Units.InvalidName",
        "The unit name cannot be empty or whitespace."
    );

    public static readonly Error InvalidFactor = Error.Problem(
        "Units.InvalidFactor",
        "Conversion factor (toBaseFactor) must be greater than zero."
    );

    public static readonly Error Conflict = Error.Conflict(
        "Units.Conflict",
        "A unit with the same name or abbreviation already exists."
    );
}
