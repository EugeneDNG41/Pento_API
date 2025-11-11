using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Units;

namespace Pento.Application.FoodReferences.Update;
public sealed record UpdateFoodReferenceCommand(
    Guid Id,
    string Name,
    string FoodGroup,
    int? FoodCategoryId,
    string? Brand,
    string? Barcode,
    string UsdaId,
    int? TypicalShelfLifeDays_Pantry,
    int? TypicalShelfLifeDays_Fridge,
    int? TypicalShelfLifeDays_Freezer,
    Uri? ImageUrl,
    string UnitType
) : ICommand<Guid>;
