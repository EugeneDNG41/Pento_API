using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.EventHandlers.Groceries;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Units;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers.FoodItems;

internal sealed class FoodItemDiscardedEventHandler(
    IActivityService activityService,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Unit> unitRepository,
    IGenericRepository<FoodItemLog> logRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<FoodItemDiscardedDomainEvent>
{
    public override async Task Handle(
        FoodItemDiscardedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(domainEvent.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            throw new PentoException(nameof(FoodItemDiscardedEventHandler), FoodItemErrors.NotFound);
        }
        Unit? unit = await unitRepository.GetByIdAsync(domainEvent.UnitId, cancellationToken);
        if (unit is null)
        {
            throw new PentoException(nameof(FoodItemDiscardedEventHandler), FoodItemErrors.InvalidMeasurementUnit);
        }
        var log = FoodItemLog.Create(
            foodItem.Id,
            foodItem.HouseholdId,
            domainEvent.UserId,
            domainEvent.Timestamp,
            FoodItemLogAction.Discard,
            domainEvent.Quantity,
            unit.Id);
        logRepository.Add(log);
        Result<UserActivity> discardResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            foodItem.HouseholdId,
            ActivityCode.FOOD_ITEM_DISCARD.ToString(),
            foodItem.Id,
            cancellationToken);
        if (discardResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), discardResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

