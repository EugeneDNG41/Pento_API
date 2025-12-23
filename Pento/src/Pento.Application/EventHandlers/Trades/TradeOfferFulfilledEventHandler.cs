using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Households;
using Pento.Domain.Notifications;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.EventHandlers.Trades;
internal sealed class TradeOfferFulfilledEventHandler(
    INotificationService notificationService,
    IActivityService activityService,  
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<Household> householdRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<TradeOfferFulfilledDomainEvent>
{
    public override async Task Handle(
        TradeOfferFulfilledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(domainEvent.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeOfferFulfilledEventHandler), TradeErrors.OfferNotFound);
        }
        Result createResult = await activityService.RecordHouseholdActivityAsync(
            offer.HouseholdId,
            ActivityCode.TRADE_COMPLETE.ToString(),
            offer.Id,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(TradeOfferCreatedEventHandler), createResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);

        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (request == null) 
        {
            throw new PentoException(nameof(TradeOfferFulfilledEventHandler), TradeErrors.RequestNotFound);
        }
        Household? requestHousehold = await householdRepository.GetByIdAsync(request.HouseholdId, cancellationToken);
        if (requestHousehold == null)
        {
            throw new PentoException(nameof(TradeOfferFulfilledEventHandler), HouseholdErrors.NotFound);
        }
        
        string title = "Trade Fulfilled";
        string body = $"Your trade with household {requestHousehold.Name} has been fulfilled.";
        var payload = new Dictionary<string, string>
        {
            { "tradeOfferId", offer.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToHouseholdAsync(
            offer.HouseholdId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(TradeOfferFulfilledEventHandler), notificationResult.Error);
        }
        await hubContext.Clients.All
            .TradeOfferFulfilled(offer.Id);
    }
}
