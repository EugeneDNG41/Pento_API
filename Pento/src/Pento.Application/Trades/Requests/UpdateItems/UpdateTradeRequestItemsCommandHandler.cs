using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Requests.UpdateItems;

internal sealed class UpdateTradeRequestItemsCommandHandler(
    IUserContext userContext,
    TradeService tradeService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeItemRequest> tradeItemRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateTradeRequestItemsCommand>
{
    public async Task<Result> Handle(UpdateTradeRequestItemsCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            return Result.Failure(TradeErrors.RequestNotFound);
        }
        if (request.HouseholdId != householdId)
        {
            return Result.Failure(TradeErrors.RequestForbiddenAccess);
        }
        if (request.Status != TradeRequestStatus.Pending)
        {
            return Result.Failure(TradeErrors.InvalidRequestState);
        }
        var affectedFoodItems = new Dictionary<Guid, decimal>();
        var affectedTradeItems = new List<TradeItemRequest>();
        foreach (UpdateTradeItemDto dto in command.Items)
        {
            TradeItemRequest? item = await tradeItemRepository.GetByIdAsync(dto.TradeItemId, cancellationToken);
            if (item is null)
            {
                return Result.Failure(TradeErrors.NotFound);
            }
            if (item.RequestId != request.Id)
            {
                return Result.Failure(TradeErrors.ItemForbiddenAccess);
            }
            item.Update(dto.Quantity, dto.UnitId);
            FoodItem? foodItem = await foodItemRepository.GetByIdAsync(item.FoodItemId, cancellationToken);
            if (foodItem is null)
            {
                return Result.Failure(FoodItemErrors.NotFound);
            }
            if (foodItem.HouseholdId != householdId.Value)
            {
                return Result.Failure(FoodItemErrors.ForbiddenAccess);
            }
            Result reconciliationResult = await tradeService
                .ReconcileTradeItemsAddedOrUpdatedOutsideSessionAsync(request.TradeOfferId, request.Id, userContext.UserId, item, foodItem, cancellationToken);
            if (reconciliationResult.IsFailure)
            {
                return Result.Failure(reconciliationResult.Error);
            }           
            affectedFoodItems.Add(foodItem.Id, foodItem.Quantity);
            affectedTradeItems.Add(item);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (TradeItemRequest item in affectedTradeItems)
        {
            await hubContext.Clients.Group(householdId.Value.ToString())
                .TradeItemUpdated(item.Id, item.Quantity, item.UnitId);
        }
        foreach (KeyValuePair<Guid, decimal> affectedItem in affectedFoodItems)
        {
            await hubContext.Clients.Group(householdId.Value.ToString())
                .FoodItemQuantityUpdated(affectedItem.Key, affectedItem.Value);
        }
        return Result.Success();
    }
}
