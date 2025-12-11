using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.Update;

public sealed record UpdateFoodItemCommand(
    Guid Id,
    Guid? StorageId,
    Guid? CompartmentId,
    Guid? UnitId,
    string? Name,
    decimal? Quantity,
    DateOnly? ExpirationDate,
    string? Notes) : ICommand;

