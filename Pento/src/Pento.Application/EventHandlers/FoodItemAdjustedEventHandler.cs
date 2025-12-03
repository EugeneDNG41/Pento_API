using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Units;

namespace Pento.Application.EventHandlers;

internal sealed class FoodItemAdjustedEventHandler(
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Unit> unitRepository,
    IGenericRepository<FoodItemLog> logRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<FoodItemQuantityAdjustedDomainEvent>
{
    public override async Task Handle(
        FoodItemQuantityAdjustedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(domainEvent.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            throw new PentoException(nameof(FoodItemAdjustedEventHandler), FoodItemErrors.NotFound);
        }
        Unit? unit = await unitRepository.GetByIdAsync(domainEvent.UnitId, cancellationToken);
        if (unit is null)
        {
            throw new PentoException(nameof(FoodItemAdjustedEventHandler), FoodItemErrors.InvalidMeasurementUnit);
        }
        if (domainEvent.Quantity == 0)
        {
            return;
        }
        var log = FoodItemLog.Create(
            foodItem.Id,
            foodItem.HouseholdId,
            domainEvent.UserId,
            domainEvent.Timestamp,
            domainEvent.Quantity > 0 ? FoodItemLogAction.Intake : FoodItemLogAction.Discard,
            Math.Abs(domainEvent.Quantity),
            unit.Id);
        logRepository.Add(log);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

