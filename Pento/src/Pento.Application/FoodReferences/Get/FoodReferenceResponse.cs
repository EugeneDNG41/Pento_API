using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.FoodReferences.Get;
public sealed record FoodReferenceResponse(
    Guid Id,
    string Name,
    string FoodGroup,
    string DataType,                    
    string? Notes,                      
    int TypicalShelfLifeDays_Pantry,
    int TypicalShelfLifeDays_Fridge,
    int TypicalShelfLifeDays_Freezer,
    Guid? AddedBy,
    Uri? ImageUrl,
    string? Brand,
    string? Barcode,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
