using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Requests.RemoveItems;

internal sealed class RemoveTradeRequestItemsCommandHandler(
    IUserContext userContext,
    IConverterService converterService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeItemRequest> tradeItemRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveTradeRequestItemsCommand>
{
    public async Task<Result> Handle(RemoveTradeRequestItemsCommand command, CancellationToken cancellationToken)
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
        IEnumerable<TradeItemRequest> items = await tradeItemRepository.FindAsync(
            item => item.RequestId == command.RequestId
                && command.TradeItemIds.Contains(item.Id),
            cancellationToken);
        var restoredItems = new Dictionary<Guid, decimal>();
        foreach (TradeItemRequest item in items)
        {
            FoodItem? foodItem = await foodItemRepository.GetByIdAsync(item.FoodItemId, cancellationToken);
            if (foodItem == null)
            {
                return Result.Failure(FoodItemErrors.NotFound);
            }
            if (foodItem.HouseholdId != householdId)
            {
                return Result.Failure(FoodItemErrors.ForbiddenAccess);
            }
            Result<decimal> restoredResult = await converterService
                .ConvertAsync(
                    item.Quantity,
                    item.UnitId,
                    foodItem.UnitId,
                    cancellationToken);
            if (restoredResult.IsFailure)
            {
                return Result.Failure(restoredResult.Error);
            }
            foodItem.AdjustReservedQuantity(restoredResult.Value);
            foodItemRepository.Update(foodItem);
            tradeItemRepository.Remove(item);
            restoredItems.Add(foodItem.Id, foodItem.Quantity);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.All.TradeItemsRemoved(command.TradeItemIds);
        foreach (KeyValuePair<Guid, decimal> restoredItem in restoredItems)
        {
            await hubContext.Clients.Group(householdId.Value.ToString())
                .FoodItemQuantityUpdated(restoredItem.Key, restoredItem.Value);
        }
        return Result.Success();
    }
}
