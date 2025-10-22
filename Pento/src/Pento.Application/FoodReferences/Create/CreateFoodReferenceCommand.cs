using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Create;
public sealed record CreateFoodReferenceCommand(
    string Name,
    FoodGroup FoodGroup,
    FoodDataType DataType,
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

