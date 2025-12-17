using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Application.Abstractions.Services;

public interface ITradeService
{
    Task<Result> ReconcileAddedTradeItemsDuringSessionAsync(TradeSession session, TradeSessionItem sessionItem, FoodItem foodItem, Guid userId, CancellationToken cancellationToken);
    Task<Result> ReconcileTradeItemsAddedOrUpdatedOutsideSessionAsync(Guid offerId, Guid? requestId, Guid userId, TradeItem tradeItem, FoodItem foodItem, CancellationToken cancellationToken);
    Task<Result> ReconcileTradeItemsRemovedFromSessionAsync(TradeSession session, TradeSessionItem sessionItem, FoodItem foodItem, CancellationToken cancellationToken);
    Task<Result> ReconcileUpdatedTradeItemsDuringSessionAsync(TradeSession session, TradeSessionItem sessionItem, FoodItem foodItem, decimal newQuantity, Guid newUnitId, CancellationToken cancellationToken);
}