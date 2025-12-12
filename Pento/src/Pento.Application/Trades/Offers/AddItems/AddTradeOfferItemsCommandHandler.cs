using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Trades;
using Pento.Domain.Units;

namespace Pento.Application.Trades.Offers.AddItems;

internal sealed class AddTradeOfferItemsCommandHandler(IUserContext userContext,
    TradeService tradeService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeItemOffer> tradeItemRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<Unit> unitRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddTradeOfferItemsCommand, IReadOnlyList<TradeItemResponse>>
{
    public async Task<Result<IReadOnlyList<TradeItemResponse>>> Handle(AddTradeOfferItemsCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(command.OfferId, cancellationToken);
        if (offer == null)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.OfferNotFound);
        }
        if (offer.HouseholdId != householdId)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.OfferForbiddenAccess);
        }
        if (offer.Status != TradeOfferStatus.Open)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.InvalidOfferState);
        }
        bool existingTradeItemsDuplicate = await tradeItemRepository
            .AnyAsync(ti => ti.OfferId == offer.Id && command.Items
                .Select(i => i.FoodItemId)
                .Contains(ti.FoodItemId), cancellationToken);
        if (existingTradeItemsDuplicate)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.DuplicateTradeItems);
        }
        int currentOfferItemCount = await tradeItemRepository.CountAsync(
            ti => ti.OfferId == offer.Id,
            cancellationToken);
        if (currentOfferItemCount + command.Items.Count > 5)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.ExceedsMaxTradeItems);
        }
        var responses = new List<TradeItemResponse>();
        var affectedFoodItems = new Dictionary<Guid, decimal>();
        foreach (AddTradeItemDto dto in command.Items)
        {
            var item = TradeItemOffer.Create(
               foodItemId: dto.FoodItemId,
               quantity: dto.Quantity,
               unitId: dto.UnitId,
               offerId: offer.Id
           );
            FoodItem? foodItem = await foodItemRepository.GetByIdAsync(dto.FoodItemId, cancellationToken);
            if (foodItem is null)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(FoodItemErrors.NotFound);
            }
            if (foodItem.HouseholdId != householdId.Value)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(FoodItemErrors.ForbiddenAccess);
            }
            Unit? unit = await unitRepository.GetByIdAsync(item.UnitId, cancellationToken);
            if (unit == null)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(UnitErrors.NotFound);
            }
            FoodReference? foodReference =
                await foodReferenceRepository.GetByIdAsync(foodItem.FoodReferenceId, cancellationToken);
            if (foodReference is null)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(FoodReferenceErrors.NotFound);
            }
            Result reconciliationResult = await tradeService
                .ReconcileTradeItemsAddedOrUpdatedOutsideSessionAsync(offer.Id, null, userContext.UserId, item, foodItem, cancellationToken);
            if (reconciliationResult.IsFailure)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(reconciliationResult.Error);
            }
            tradeItemRepository.Add(item);
            responses.Add(new TradeItemResponse(
                item.Id,
                foodItem.Id,
                foodItem.Name,
                foodReference.Name,
                foodItem.ImageUrl,
                foodReference.FoodGroup.ToReadableString(),
                item.Quantity,
                unit.Abbreviation,
                item.UnitId,
                foodItem.ExpirationDate,
                item.From));
            affectedFoodItems.Add(foodItem.Id, foodItem.Quantity);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.Group(householdId.Value.ToString())
            .TradeOfferItemsAdded(offer.Id, responses);
        foreach (KeyValuePair<Guid, decimal> affectedItem in affectedFoodItems)
        {
            await hubContext.Clients.Group(householdId.Value.ToString())
                .FoodItemQuantityUpdated(affectedItem.Key, affectedItem.Value);
        }
        return responses;
    }
}
