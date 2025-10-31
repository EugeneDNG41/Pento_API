using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Create;

public sealed record CreateFoodItemCommand(
    Guid FoodRefId, 
    Guid CompartmentId,
    Guid? HouseholdId,
    string? CustomName, 
    decimal Quantity, 
    Guid UnitId, 
    DateTime ExpirationDate, 
    string? Notes) : ICommand<Guid>;
