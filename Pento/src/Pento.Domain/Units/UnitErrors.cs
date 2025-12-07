using Pento.Domain.Abstractions;

namespace Pento.Domain.Units;

public static class UnitErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Units.NotFound", $"Measurement unit not found.");

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

    public static readonly Error InvalidConversion = Error.Problem(
        "Units.InvalidConversion",
        "Cannot convert between units of different types."
    );
}
