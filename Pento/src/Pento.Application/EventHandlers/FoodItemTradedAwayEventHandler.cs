using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.FoodItems;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers;

internal sealed class FoodItemTradedAwayEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<FoodItemLog> logRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<FoodItemTradedAwayDomainEvent>
{
    public override async Task Handle(
        FoodItemTradedAwayDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(domainEvent.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            throw new PentoException(nameof(FoodItemTradedAwayEventHandler), FoodItemErrors.NotFound);
        }
        var log = FoodItemLog.Create(
            foodItem.Id,
            foodItem.HouseholdId,
            domainEvent.UserId,
            domainEvent.Timestamp,
            FoodItemLogAction.TradeAway,
            domainEvent.Quantity,
            domainEvent.UnitId);
        logRepository.Add(log);
        Result<UserActivity> tradeResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            foodItem.HouseholdId,
            ActivityCode.FOOD_ITEM_TRADE_AWAY.ToString(),
            foodItem.Id,
            cancellationToken);
        if (tradeResult.IsFailure)
        {
            throw new PentoException(nameof(FoodItemTradedAwayEventHandler), tradeResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(tradeResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(FoodItemTradedAwayEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
