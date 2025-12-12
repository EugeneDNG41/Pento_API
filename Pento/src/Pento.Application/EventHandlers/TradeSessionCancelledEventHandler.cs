using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Application.EventHandlers;

internal sealed class TradeSessionCancelledEventHandler(
    TradeService tradeService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeSessionItem> tradeItemSessionRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork
    )
    : DomainEventHandler<TradeSessionCancelledDomainEvent>
{
    public override async Task Handle(
        TradeSessionCancelledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(domainEvent.TradeSessionId, cancellationToken);
        if (session == null)
        {
            throw new PentoException(nameof(TradeSessionCancelledEventHandler), TradeErrors.SessionNotFound);
        }
        IEnumerable<TradeSessionItem> sessionItems = await tradeItemSessionRepository.FindAsync(
            tis => tis.SessionId == session.Id,
            cancellationToken);
        foreach (TradeSessionItem sessionItem in sessionItems)
        {
            FoodItem? foodItem = await foodItemRepository.GetByIdAsync(sessionItem.FoodItemId, cancellationToken);
            if (foodItem == null)
            {
                throw new PentoException(nameof(TradeSessionCancelledEventHandler), FoodItemErrors.NotFound);
            }
            Result release = await tradeService.ReconcileTradeItemsRemovedFromSessionAsync(session, sessionItem, foodItem, cancellationToken);
            if (release.IsFailure)
            {
                throw new PentoException(nameof(TradeSessionCancelledEventHandler), release.Error);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
