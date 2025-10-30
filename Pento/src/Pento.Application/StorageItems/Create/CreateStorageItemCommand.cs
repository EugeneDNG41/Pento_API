using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.StorageItems.Create;

public sealed record CreateStorageItemCommand(
    Guid FoodRefId, 
    Guid CompartmentId, 
    string? CustomName, 
    decimal Quantity, 
    Guid UnitId, 
    DateTime ExpirationDate, 
    string? Notes) : ICommand<Guid>;
