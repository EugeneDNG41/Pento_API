using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Notifications;
using Pento.Domain.Trades;
using Pento.Domain.Units;
using Pento.Domain.UserActivities;
using Pento.Domain.Users;

namespace Pento.Application.EventHandlers;

internal sealed class FoodItemAddedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Unit> unitRepository,
    IGenericRepository<FoodItemLog> logRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<FoodItemAddedDomainEvent>
{
    public override async Task Handle(
        FoodItemAddedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(domainEvent.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            throw new PentoException(nameof(FoodItemAddedEventHandler), FoodItemErrors.NotFound);
        }
        Unit? unit = await unitRepository.GetByIdAsync(domainEvent.UnitId, cancellationToken);
        if (unit is null)
        {
            throw new PentoException(nameof(FoodItemAddedEventHandler), FoodItemErrors.InvalidMeasurementUnit);
        }
        var log = FoodItemLog.Create(
            foodItem.Id,
            foodItem.HouseholdId,
            domainEvent.UserId,
            domainEvent.Timestamp,
            FoodItemLogAction.Intake,
            domainEvent.Quantity,
            unit.Id);
        logRepository.Add(log);
        Result<UserActivity> intakeResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            foodItem.HouseholdId,
            ActivityCode.FOOD_ITEM_INTAKE.ToString(),
            foodItem.Id,
            cancellationToken);
        if (intakeResult.IsFailure)
        {
            throw new PentoException(nameof(FoodItemAddedEventHandler), intakeResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(intakeResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class TradeSessionCreatedEventHandler(
    INotificationService notificationService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<User> userRepository)
    : DomainEventHandler<TradeSessionCreatedDomainEvent>
{
    public override async Task Handle(
        TradeSessionCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(domainEvent.TradeSessionId, cancellationToken);
        if (session == null)
        {
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), TradeErrors.SessionNotFound);
        }
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(session.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), TradeErrors.OfferNotFound);
        }
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(session.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), TradeErrors.RequestNotFound);
        }
        User? offerUser = await userRepository.GetByIdAsync(offer.UserId, cancellationToken);
        if (offerUser == null)
        {
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), UserErrors.NotFound);
        }
        string title = "Trade Session Opened";
        string body = $"{offerUser.FirstName} has opened a trade session for your offer.";
        var payload = new Dictionary<string, string>
        {
            { "tradeSessionId", session.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToUserAsync(
            request.UserId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), notificationResult.Error);
        }
        await hubContext.Clients.User(request.UserId.ToString())
            .TradeSessionOpened(session.Id);
    }
}
internal sealed class TradeRequestCreatedEventHandler(
    INotificationService notificationService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<User> userRepository)
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
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(request.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), TradeErrors.OfferNotFound);
        }
        User? requestUser = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (requestUser == null)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), UserErrors.NotFound);
        }
        string title = "New Trade Request";
        string body = $"you have received a new trade request from {requestUser.FirstName}.";
        var payload = new Dictionary<string, string>
        {
            { "tradeRequestId", request.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToUserAsync(
            offer.UserId,
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
internal sealed class TradeSessionMessageCreatedEventHandler(
    INotificationService notificationService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeSessionMessage> tradeSessionMessageRepository,
    IGenericRepository<User> userRepository)
    : DomainEventHandler<TradeSessionMessageCreatedDomainEvent>
{
    public override async Task Handle(
        TradeSessionMessageCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeSessionMessage? message = await tradeSessionMessageRepository.GetByIdAsync(domainEvent.TradeSessionMessageId, cancellationToken);
        if (message == null)
        {
            throw new PentoException(nameof(TradeSessionMessageCreatedEventHandler), TradeErrors.MessageNotFound);
        }
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(message.TradeSessionId, cancellationToken);
        if (session == null)
        {
            throw new PentoException(nameof(TradeSessionMessageCreatedEventHandler), TradeErrors.SessionNotFound);
        }      
        User? senderUser = await userRepository.GetByIdAsync(message.UserId, cancellationToken);
        if (senderUser == null)
        {
            throw new PentoException(nameof(TradeSessionMessageCreatedEventHandler), UserErrors.NotFound);
        }
        Guid recipientUserId = session.OfferUserId == message.UserId
            ? session.RequestUserId
            : session.OfferUserId;
        string title = $"New Message From {senderUser.FirstName}";
        string body = $"{senderUser.FirstName}: {message.MessageText}.";
        var payload = new Dictionary<string, string>
        {
            { "tradeSessionId", session.Id.ToString() },
            { "tradeSessionMessageId", message.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToUserAsync(
            recipientUserId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(TradeSessionMessageCreatedEventHandler), notificationResult.Error);
        }
    }
}
internal sealed class TradeOfferCancelledEventHandler(
    INotificationService notificationService,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<User> userRepository)
    : DomainEventHandler<TradeOfferCancelledDomainEvent>
{
    public override async Task Handle(
        TradeOfferCancelledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(domainEvent.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeOfferCancelledEventHandler), TradeErrors.OfferNotFound);
        }
        User? offerUser = await userRepository.GetByIdAsync(offer.UserId, cancellationToken);
        if (offerUser == null)
        {
            throw new PentoException(nameof(TradeOfferCancelledEventHandler), UserErrors.NotFound);
        }
        IEnumerable<TradeRequest> requests = await tradeRequestRepository.FindAsync(
            r => r.TradeOfferId == offer.Id && r.Status == TradeRequestStatus.Pending,
            cancellationToken);
        string title = "Trade Offer Cancelled";
        string body = $"{offerUser.FirstName} has cancelled their trade offer(s)";
        var payload = new Dictionary<string, string>
            {
                { "tradeOfferId", offer.Id.ToString() }
            };
        foreach (TradeRequest request in requests)
        {
            request.AutoCancel();
            payload.Add("tradeRequestId", request.Id.ToString());
            Result notificationResult = await notificationService.SendToUserAsync(
                request.UserId,
                title,
                body,
                NotificationType.Trade,
                payload,
                cancellationToken);
            if (notificationResult.IsFailure)
            {
                throw new PentoException(nameof(TradeOfferCancelledEventHandler), notificationResult.Error);
            }
        }
    }
}
internal sealed class TradeRequestCancelledEventHandler(
    INotificationService notificationService,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<User> userRepository)
    : DomainEventHandler<TradeRequestCancelledDomainEvent>
{
    public override async Task Handle(
        TradeRequestCancelledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeRequestCancelledEventHandler), TradeErrors.RequestNotFound);
        }
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(request.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeRequestCancelledEventHandler), TradeErrors.OfferNotFound);
        }
        User? requestUser = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (requestUser == null)
        {
            throw new PentoException(nameof(TradeRequestCancelledEventHandler), UserErrors.NotFound);
        }
        string title = "Trade Request Cancelled";
        string body = $"{requestUser.FirstName} has cancelled their trade request.";
        var payload = new Dictionary<string, string>
            {
                { "tradeOfferId", offer.Id.ToString() },
                { "tradeRequestId", request.Id.ToString() }
            };
        Result notificationResult = await notificationService.SendToUserAsync(
            offer.UserId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(TradeRequestCancelledEventHandler), notificationResult.Error);
        }
    }
}
internal sealed class TradeRequestRejectedEventHandler(
    INotificationService notificationService,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<User> userRepository)
     : DomainEventHandler<TradeRequestRejectedDomainEvent>
{
    public async override Task Handle(TradeRequestRejectedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeRequestRejectedEventHandler), TradeErrors.RequestNotFound);
        }
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(request.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeRequestRejectedEventHandler), TradeErrors.OfferNotFound);
        }
        User? offerUser = await userRepository.GetByIdAsync(offer.UserId, cancellationToken);
        if (offerUser == null)
        {
            throw new PentoException(nameof(TradeRequestRejectedEventHandler), UserErrors.NotFound);
        }
        string title = "Trade Request Rejected";
        string body = $"{offerUser.FirstName} has rejected your trade request.";
        var payload = new Dictionary<string, string>
            {
                { "tradeRequestId", request.Id.ToString() },
                { "tradeOfferId", offer.Id.ToString() }
            };
        Result notificationResult = await notificationService.SendToUserAsync(
            request.UserId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(TradeRequestRejectedEventHandler), notificationResult.Error);
        }
    }
}
internal sealed class TradeSessionCompletedEventHandler(
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,  
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeItemOffer> tradeItemOfferRepository,
    IGenericRepository<TradeItemRequest> tradeItemRequestRepository,
    IGenericRepository<TradeItemSession> tradeItemSessionRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<TradeSessionCompletedDomainEvent>
{
    public override async Task Handle(
        TradeSessionCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(domainEvent.TradeSessionId, cancellationToken);
        if (session == null)
        {
            throw new PentoException(nameof(TradeSessionCompletedEventHandler), TradeErrors.SessionNotFound);
        }
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(session.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeSessionCompletedEventHandler), TradeErrors.OfferNotFound);
        }
        
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(session.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeSessionCompletedEventHandler), TradeErrors.RequestNotFound);
        }
        offer.Fulfill(session.TradeRequestId);
        request.Fulfill(session.TradeOfferId);
        IEnumerable<TradeItemOffer> offeredItems = await tradeItemOfferRepository.FindAsync(
            tio => tio.OfferId == offer.Id,
            cancellationToken);
        IEnumerable<TradeItemRequest> requestedItems = await tradeItemRequestRepository.FindAsync(
            tir => tir.RequestId == request.Id,
            cancellationToken);
        IEnumerable<TradeItemSession> sessionItems = await tradeItemSessionRepository.FindAsync(
            tis => tis.SessionId == session.Id,
            cancellationToken);
        foreach (TradeItemOffer offeredItem in offeredItems) //remove any not in session item
        {
            bool inSession = sessionItems.Any(si => si.ItemFrom == TradeItemSessionFrom.Offer && si.FoodItemId == offeredItem.FoodItemId);
            if (!inSession)
            {
                tradeItemOfferRepository.Remove(offeredItem);
            }
        }
        foreach (TradeItemRequest requestedItem in requestedItems) //remove any not in session item
        {
            bool inSession = sessionItems.Any(si => si.ItemFrom == TradeItemSessionFrom.Request && si.FoodItemId == requestedItem.FoodItemId);
            if (!inSession)
            {
                tradeItemRequestRepository.Remove(requestedItem);
            }
        }
        
        foreach (TradeItemSession sessionItem in sessionItems) //overwrite offered/requested quantities with actual traded quantities
        {
            if (sessionItem.ItemFrom == TradeItemSessionFrom.Offer)
            {
                TradeItemOffer? offeredItem = offeredItems.SingleOrDefault(oi => oi.FoodItemId == sessionItem.FoodItemId); //business rule: only one offered item per food item id
                if (offeredItem != null)
                {
                    offeredItem.Update(sessionItem.Quantity, sessionItem.UnitId);
                    tradeItemOfferRepository.Update(offeredItem);
                } else
                {
                    offeredItem = TradeItemOffer.Create(
                        sessionItem.FoodItemId,
                        sessionItem.Quantity,
                        sessionItem.UnitId,
                        offer.Id);
                    tradeItemOfferRepository.Add(offeredItem);
                }
            }
            else if (sessionItem.ItemFrom == TradeItemSessionFrom.Request)
            {
                TradeItemRequest? requestedItem = requestedItems.FirstOrDefault(ri => ri.FoodItemId == sessionItem.FoodItemId);
                if (requestedItem != null)
                {
                    requestedItem.Update(sessionItem.Quantity, sessionItem.UnitId);
                    tradeItemRequestRepository.Update(requestedItem);
                }
                else
                {
                    requestedItem = TradeItemRequest.Create(
                        sessionItem.FoodItemId,
                        sessionItem.Quantity,
                        sessionItem.UnitId,
                        request.Id);
                    tradeItemRequestRepository.Add(requestedItem);
                }
            } 
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class TradeOfferFulfilledEventHandler(
    INotificationService notificationService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<User> userRepository)
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
        User? requestUser = await userRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (requestUser == null)
        {
            throw new PentoException(nameof(TradeOfferFulfilledEventHandler), UserErrors.NotFound);
        }
        string title = "Trade Fulfilled";
        string body = $"Your trade with {requestUser.FirstName} has been fulfilled.";
        var payload = new Dictionary<string, string>
        {
            { "tradeOfferId", offer.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToUserAsync(
            offer.UserId,
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
