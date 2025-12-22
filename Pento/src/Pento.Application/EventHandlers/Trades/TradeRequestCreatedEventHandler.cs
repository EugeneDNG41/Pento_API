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
using Pento.Domain.UserActivities;
using Pento.Domain.Users;

namespace Pento.Application.EventHandlers.Trades;

internal sealed class TradeRequestCreatedEventHandler(
    INotificationService notificationService,
    IActivityService activityService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<Household> householdRepository,
    IUnitOfWork unitOfWork
    )
    : DomainEventHandler<TradeRequestCreatedDomainEvent>
{
    public override async Task Handle(
        TradeRequestCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), TradeErrors.RequestNotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            request.UserId,
            request.HouseholdId,
            ActivityCode.TRADE_REQUEST_CREATE.ToString(),
            request.Id,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), createResult.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(request.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), TradeErrors.OfferNotFound);
        }
        Household? requestHousehold = await householdRepository.GetByIdAsync(offer.HouseholdId, cancellationToken);
        if (requestHousehold == null)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), HouseholdErrors.NotFound);
        }
        string title = "New Trade Request";
        string body = $"you have received a new trade offer from household {requestHousehold.Name}.";
        var payload = new Dictionary<string, string>
        {
            { "tradeRequestId", request.Id.ToString() }
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
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), notificationResult.Error);
        }
        await hubContext.Clients.User(offer.UserId.ToString())
            .TradeRequestCreated(request.Id, offer.Id);
    }
}
