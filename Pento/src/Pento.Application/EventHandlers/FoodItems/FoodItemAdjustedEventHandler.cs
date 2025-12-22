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

internal sealed class FoodItemAdjustedEventHandler(
    IActivityService activityService,
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
        Result<UserActivity> adjustResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            foodItem.HouseholdId,
            domainEvent.Quantity > 0 ? ActivityCode.FOOD_ITEM_INTAKE.ToString() : ActivityCode.FOOD_ITEM_DISCARD.ToString(),
            foodItem.Id,
            cancellationToken);
        if (adjustResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), adjustResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

