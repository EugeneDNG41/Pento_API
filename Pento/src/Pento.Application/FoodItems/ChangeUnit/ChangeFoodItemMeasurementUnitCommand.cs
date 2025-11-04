using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.ChangeUnit;

public sealed record ChangeFoodItemMeasurementUnitCommand(Guid Id, Guid MeasurementUnitId, int Version) : ICommand;
