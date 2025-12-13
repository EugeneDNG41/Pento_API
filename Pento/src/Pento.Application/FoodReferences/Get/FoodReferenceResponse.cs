using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Get;

public sealed record FoodReferenceResponse(
    Guid Id,
    string Name,
    string FoodGroup,
    int TypicalShelfLifeDays_Pantry,
    int TypicalShelfLifeDays_Fridge,
    int TypicalShelfLifeDays_Freezer,
    Guid? AddedBy,
    Uri? ImageUrl,
    string? Brand,
    string? Barcode,
    string UnitType,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
public sealed record FoodReferenceRow(
    Guid Id,
    string Name,
    FoodGroup FoodGroup,
    int TypicalShelfLifeDays_Pantry,
    int TypicalShelfLifeDays_Fridge,
    int TypicalShelfLifeDays_Freezer,
    Guid? AddedBy,
    Uri? ImageUrl,
    string? Brand,
    string? Barcode,
    string UnitType,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
