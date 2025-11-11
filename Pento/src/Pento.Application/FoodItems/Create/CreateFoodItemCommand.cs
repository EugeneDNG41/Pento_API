using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Create;

public sealed record CreateFoodItemCommand(
    Guid FoodReferenceId, 
    Guid CompartmentId,
    string? Name, 
    decimal Quantity, 
    Guid? UnitId, 
    DateOnly? ExpirationDate, 
    string? Notes) : ICommand<Guid>;
