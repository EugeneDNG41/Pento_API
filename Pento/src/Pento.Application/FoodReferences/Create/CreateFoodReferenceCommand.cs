using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodReferences.Create;
public sealed record CreateFoodReferenceCommand
    (string Name,
    string FoodGroup,
    string? Barcode,
    string? Brand,
    int TypicalShelfLifeDays,
    string? OpenFoodFactsId,
    string? UsdaId
    ) : ICommand<Guid>;

