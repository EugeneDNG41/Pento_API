using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;

namespace Pento.Application.FoodReferences.Create;
public sealed record CreateFoodReferenceCommand(
    string Name,
    FoodGroup FoodGroup,
    int? FoodCategoryId,
    string? Brand,
    string? Barcode,
    string UsdaId,
    int? TypicalShelfLifeDays_Pantry,
    int? TypicalShelfLifeDays_Fridge,
    int? TypicalShelfLifeDays_Freezer,
    Guid? AddedBy,
    Uri? ImageUrl,
    UnitType UnitType
) : ICommand<Guid>;

