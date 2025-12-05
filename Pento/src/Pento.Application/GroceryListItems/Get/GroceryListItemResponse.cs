namespace Pento.Application.GroceryListItems.Get;

public sealed record GroceryListItemResponse(
    Guid Id,
    Guid ListId,
    Guid FoodRefId,
    string? CustomName,
    decimal Quantity,
    Guid? UnitId,
    string? Notes,
    string Priority,
    Guid AddedBy,
    DateTime CreatedOnUtc,
    string FoodGroup,
    int TypicalShelfLifeDays_Pantry,
    int TypicalShelfLifeDays_Fridge,
    int TypicalShelfLifeDays_Freezer,
    string FoodName,
    Uri? FoodImageUri
);
