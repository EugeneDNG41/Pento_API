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
    string? Barcode,
    string? Brand,
    int TypicalShelfLifeDays,
    string? OpenFoodFactsId,
    string? UsdaId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
