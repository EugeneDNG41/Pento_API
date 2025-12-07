namespace Pento.Domain.Units;

public static class UnitData
{
    //Weight-based
    public static readonly Unit Gram = Unit.Create(
        name: "Gram",
        abbreviation: "g",
        toBaseFactor: 1m,
        type: UnitType.Weight);

    public static readonly Unit Kilogram = Unit.Create(
        name: "Kilogram",
        abbreviation: "kg",
        toBaseFactor: 1000m,
        type: UnitType.Weight);

    public static readonly Unit Ounce = Unit.Create(
        name: "Ounce",
        abbreviation: "oz",
        toBaseFactor: 28.3495m,
        type: UnitType.Weight);

    public static readonly Unit Pound = Unit.Create(
        name: "Pound",
        abbreviation: "lb",
        toBaseFactor: 453.592m,
        type: UnitType.Weight);


    //Volume-based
    public static readonly Unit Millilitre = Unit.Create(
        name: "Millilitre",
        abbreviation: "mL",
        toBaseFactor: 1m,
        type: UnitType.Volume);

    public static readonly Unit Litre = Unit.Create(
        name: "Litre",
        abbreviation: "L",
        toBaseFactor: 1000m,
        type: UnitType.Volume);

    public static readonly Unit USFluidOunce = Unit.Create(
        name: "Fluid ounce (US)",
        abbreviation: "fl oz",
        toBaseFactor: 29.574m,
        type: UnitType.Volume);

    public static readonly Unit USCup = Unit.Create(
        name: "Cup (US)",
        abbreviation: "cup (US)",
        toBaseFactor: 240m,
        type: UnitType.Volume);

    public static readonly Unit USPint = Unit.Create(
        name: "Pint (US)",
        abbreviation: "pt",
        toBaseFactor: 473.2m,
        type: UnitType.Volume);

    public static readonly Unit USQuart = Unit.Create(
        name: "Quart (US)",
        abbreviation: "qt",
        toBaseFactor: 946.35m,
        type: UnitType.Volume);

    public static readonly Unit USGallon = Unit.Create(
        name: "Gallon (US)",
        abbreviation: "gal",
        toBaseFactor: 3785.4m,
        type: UnitType.Volume);

    public static readonly Unit TeaspoonUS = Unit.Create(
        name: "Teaspoon (US)",
        abbreviation: "tsp",
        toBaseFactor: 5m, //(21 CFR 101.9)
        type: UnitType.Volume);

    public static readonly Unit TablespoonUS = Unit.Create(
        name: "Tablespoon (US)",
        abbreviation: "Tbsp",
        toBaseFactor: 15m, //(21 CFR 101.9)
        type: UnitType.Volume);

    //Usage-based
    public static readonly Unit Piece = Unit.Create(
        name: "Piece",
        abbreviation: "pc",
        toBaseFactor: 1m, // base: 1 pc = 1 pc
        type: UnitType.Count);

    public static readonly Unit Serving = Unit.Create(
        name: "Serving",
        abbreviation: "serving",
        toBaseFactor: 1m, // baseline: 1 serving = 1 unit of consumption
        type: UnitType.Count);


    public static readonly Unit Pair = Unit.Create(
        name: "Pair",
        abbreviation: "pair",
        toBaseFactor: 2m, // 1 pair = 2 pieces
        type: UnitType.Count);

    public static readonly Unit Dozen = Unit.Create(
        name: "Dozen",
        abbreviation: "doz",
        toBaseFactor: 12m, // 1 dozen = 12 pieces
        type: UnitType.Count);

}
