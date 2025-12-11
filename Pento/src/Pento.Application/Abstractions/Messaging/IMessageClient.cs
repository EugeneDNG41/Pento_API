using Pento.Application.FoodItems.Delete;
using Pento.Application.FoodItems.Search;
using Pento.Application.FoodItems.Update;
using Pento.Application.Trades.Sessions.AddItems;
using Pento.Application.Trades.Sessions.SendMessage;

namespace Pento.Application.Abstractions.Messaging;

public interface IMessageClient
{
    Task FoodItemAdded(FoodItemPreview foodItem);
    Task FoodItemUpdated(UpdateFoodItemCommand command);
    Task FoodItemQuantityUpdated(Guid foodItemId, decimal newQuantity);
    Task FoodItemDeleted(Guid foodItemId);
    Task TradeSessionOpened(Guid sessionId);
    Task TradeSessionCancelled(Guid sessionId);
    Task TradeRequestCreated(Guid requestId, Guid offerId);
    Task TradeMessageSent(TradeMessageResponse message);
    Task TradeRequestCancelled(Guid requestId);
    Task TradeRequestRejected(Guid requestId);
    Task TradeOfferCancelled(Guid offerId);
    Task TradeOfferExpired(Guid offerId);
    Task TradeSessionItemsRemoved(Guid sessionId, Guid[] tradeItemIds);
    Task TradeSessionItemsAdded(Guid sessionId, List<TradeItemResponse> tradeItems);
    Task TradeOfferFulfilled(Guid offerId);
    Task TradeRequestFulfilled(Guid requestId);
    Task TradeItemUpdated(Guid tradeItemId,  decimal quantity, Guid unitId);
}
