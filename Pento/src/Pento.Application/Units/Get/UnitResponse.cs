namespace Pento.Application.Units.Get;

public sealed record UnitResponse(
    Guid Id,
    string Name,
    string Abbreviation,
    decimal ToBaseFactor,
    string Type
);
