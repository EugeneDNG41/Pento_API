using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.AddItems;

internal sealed class AddTradeSessionItemsCommandHandler(
    IUserContext userContext,
    TradeService tradeService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeSessionItem> tradeItemSessionRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddTradeSessionItemsCommand, IReadOnlyList<TradeItemResponse>>
{
    public async Task<Result<IReadOnlyList<TradeItemResponse>>> Handle(AddTradeSessionItemsCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        Guid userId = userContext.UserId;
        if (householdId is null)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(command.TradeSessionId, cancellationToken);
        if (session == null) 
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.SessionNotFound);
        }
        if (session.OfferUserId != userId && session.RequestUserId != userId)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.InvalidSessionState);
        }
        TradeItemFrom from = session.OfferUserId == userId
                ? TradeItemFrom.Offer
                : TradeItemFrom.Request;
        
        bool existingItemsExist = await tradeItemSessionRepository
            .AnyAsync(item => item.SessionId == command.TradeSessionId 
                && command.Items.Select(i => i.FoodItemId).Contains(item.FoodItemId),
                cancellationToken);      
        if (existingItemsExist) {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.DuplicateTradeItems);
        }
        
        int currentItemCount = await tradeItemSessionRepository
            .CountAsync(item => item.SessionId == command.TradeSessionId && item.From == from, cancellationToken);
        if (command.Items.Count + currentItemCount > 5)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.ExceedsMaxTradeItems);
        }
        var responses = new List<TradeItemResponse>();
        var affectedFoodItems = new Dictionary<Guid, decimal>();
        foreach (AddTradeItemDto dto in command.Items)
        {
            var sessionItem = TradeSessionItem.Create(
                foodItemId: dto.FoodItemId,
                quantity: dto.Quantity,
                unitId: dto.UnitId,
                from,
                sessionId: session.Id);
            FoodItem? foodItem =
                await foodItemRepository.GetByIdAsync(dto.FoodItemId, cancellationToken);
            if (foodItem is null)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(FoodItemErrors.NotFound);
            }
            FoodReference? foodReference =
                await foodReferenceRepository.GetByIdAsync(foodItem.FoodReferenceId, cancellationToken);
            if (foodReference is null)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(FoodReferenceErrors.NotFound);
            }
            if (foodItem.HouseholdId != householdId)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(FoodItemErrors.ForbiddenAccess);
            }

            Result reconciliationResult = await tradeService.ReconcileRemovedTradeItemsDuringSessionAsync(
                session,
                sessionItem,
                foodItem,
                userId,
                cancellationToken);
            if (reconciliationResult.IsFailure)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(reconciliationResult.Error);
            }
            tradeItemSessionRepository.Add(sessionItem);
            responses.Add(new TradeItemResponse(
                sessionItem.Id,
                foodItem.Id,
                foodItem.Name,
                foodReference.Name,
                foodReference.FoodGroup.ToReadableString(),
                sessionItem.Quantity,
                sessionItem.UnitId,
                foodItem.ExpirationDate,
                sessionItem.From));
            affectedFoodItems.Add(foodItem.Id, foodItem.Quantity);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.Group(session.Id.ToString())
            .TradeSessionItemsAdded(session.Id, responses);
        foreach (KeyValuePair<Guid, decimal> affectedItem in affectedFoodItems)
        {
            await hubContext.Clients.Group(householdId.Value.ToString())
                .FoodItemQuantityUpdated(affectedItem.Key, affectedItem.Value);
        }
        return responses;
    }
}
