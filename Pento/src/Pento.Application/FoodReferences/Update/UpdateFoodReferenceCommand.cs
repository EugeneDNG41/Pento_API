using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.FoodReferences.Update;
public sealed record UpdateFoodReferenceCommand(
    Guid Id,
    string Name,
    string FoodGroup,
    string DataType,
    string? Notes,
    int? FoodCategoryId,
    string? Brand,
    string? Barcode,
    string UsdaId,
    DateTime PublishedOnUtc,
    int? TypicalShelfLifeDays_Pantry,
    int? TypicalShelfLifeDays_Fridge,
    int? TypicalShelfLifeDays_Freezer,
    Uri? ImageUrl
) : ICommand<Guid>;
