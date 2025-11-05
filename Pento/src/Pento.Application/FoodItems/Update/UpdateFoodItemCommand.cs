using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Internal;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Update;

public sealed record UpdateFoodItemCommand(
    Guid Id,
    Guid CompartmentId,
    Guid UnitId,
    string? Name, 
    decimal Quantity, 
    DateTime ExpirationDate, 
    string? Notes, 
    int Version) : ICommand;

