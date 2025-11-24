using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.FoodReferences.Create.Bluk;
public sealed class CreateFoodReferenceBulkRequest
{
    public string Name { get; init; } = string.Empty;
    public string FoodGroup { get; init; } = string.Empty;
    public int? FoodCategoryId { get; init; }
    public string? Brand { get; init; }
    public string? Barcode { get; init; }
    public string UsdaId { get; init; } = string.Empty;
    public int? TypicalShelfLifeDays_Pantry { get; init; }
    public int? TypicalShelfLifeDays_Fridge { get; init; }
    public int? TypicalShelfLifeDays_Freezer { get; init; }
    public Guid? AddedBy { get; init; }
    public string? ImageUrl { get; init; }
    public string UnitType { get; init; } = string.Empty;
}

