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

internal sealed class FoodItemConsumedEventHandler(
    IActivityService activityService,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Unit> unitRepository,
    IGenericRepository<FoodItemLog> logRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<FoodItemConsumedDomainEvent>
{
    public override async Task Handle(
        FoodItemConsumedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(domainEvent.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            throw new PentoException(nameof(FoodItemConsumedEventHandler), FoodItemErrors.NotFound);
        }
        Unit? unit = await unitRepository.GetByIdAsync(domainEvent.UnitId, cancellationToken);
        if (unit is null)
        {
            throw new PentoException(nameof(FoodItemConsumedEventHandler), FoodItemErrors.InvalidMeasurementUnit);
        }
        var log = FoodItemLog.Create(
            foodItem.Id,
            foodItem.HouseholdId,
            domainEvent.UserId,
            domainEvent.Timestamp,
            FoodItemLogAction.Consumption,
            domainEvent.Quantity,
            unit.Id);
        logRepository.Add(log);
        Result<UserActivity> consumptionResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            foodItem.HouseholdId,
            ActivityCode.FOOD_ITEM_CONSUME.ToString(),
            foodItem.Id,
            cancellationToken);
        if (consumptionResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), consumptionResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

