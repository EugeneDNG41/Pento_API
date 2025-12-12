using Azure.Core;
using FluentValidation;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Compartments;
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
        string title = "TradeAway Offer Cancelled";
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
        string title = "TradeAway Request Cancelled";
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
        string title = "TradeAway Request Rejected";
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
    IConverterService converterService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,  
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeItemOffer> tradeItemOfferRepository,
    IGenericRepository<TradeItemRequest> tradeItemRequestRepository,
    IGenericRepository<TradeSessionItem> tradeItemSessionRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Compartment> compartmentRepository,
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
        //restore offered/requested items back to food items before reconciling with session items
        IEnumerable<TradeItemOffer> offeredItems = await tradeItemOfferRepository.FindAsync(
            tio => tio.OfferId == offer.Id,
            cancellationToken);
        IEnumerable<TradeItemRequest> requestedItems = await tradeItemRequestRepository.FindAsync(
            tir => tir.RequestId == request.Id,
            cancellationToken);
        IEnumerable<TradeSessionItem> sessionItems = await tradeItemSessionRepository.FindAsync(
            tis => tis.SessionId == session.Id,
            cancellationToken);
        IEnumerable<FoodItem> allInvolvedFoodItems = await foodItemRepository.FindAsync(
            fi => offeredItems.Select(oi => oi.FoodItemId).Contains(fi.Id) ||
                  requestedItems.Select(ri => ri.FoodItemId).Contains(fi.Id) ||
                  sessionItems.Select(si => si.FoodItemId).Contains(fi.Id),
            cancellationToken);
        foreach (TradeItemOffer offeredItem in offeredItems) //remove any not in session item
        {
            FoodItem foodItem = allInvolvedFoodItems.Single(fi => fi.Id == offeredItem.FoodItemId);
            Result<decimal> conversionResult = await converterService.ConvertAsync(
                offeredItem.Quantity,
                offeredItem.UnitId,
                foodItem.UnitId,
                cancellationToken);
            if (conversionResult.IsFailure)
            {
                throw new PentoException(nameof(TradeSessionCompletedEventHandler), conversionResult.Error);
            }
            foodItem.AdjustReservedQuantity(conversionResult.Value, offer.UserId); //restore reserved quantity
            bool inSession = sessionItems.Any(si => si.From == TradeItemFrom.Offer && si.FoodItemId == offeredItem.FoodItemId);
            if (!inSession)
            {
                tradeItemOfferRepository.Remove(offeredItem);
            }
        }
        foreach (TradeItemRequest requestedItem in requestedItems) //remove any not in session item
        {
            FoodItem foodItem = allInvolvedFoodItems.Single(fi => fi.Id == requestedItem.FoodItemId);
            Result<decimal> conversionResult = await converterService.ConvertAsync(
                requestedItem.Quantity,
                requestedItem.UnitId,
                foodItem.UnitId,
                cancellationToken);
            if (conversionResult.IsFailure)
            {
                throw new PentoException(nameof(TradeSessionCompletedEventHandler), conversionResult.Error);
            }
            foodItem.AdjustReservedQuantity(conversionResult.Value, offer.UserId); //restore reserved quantity
            bool inSession = sessionItems.Any(si => si.From == TradeItemFrom.Request && si.FoodItemId == requestedItem.FoodItemId);
            if (!inSession)
            {
                tradeItemRequestRepository.Remove(requestedItem);
            }
        }
        
        foreach (TradeSessionItem sessionItem in sessionItems) //overwrite offered/requested quantities with actual traded quantities
        {
            if (sessionItem.From == TradeItemFrom.Offer)
            {
                TradeItemOffer? offeredItem = offeredItems.SingleOrDefault(oi => oi.FoodItemId == sessionItem.FoodItemId); //business rule: only one offered item per food item id
                if (offeredItem != null)
                {
                    FoodItem foodItem = allInvolvedFoodItems.Single(fi => fi.Id == offeredItem.FoodItemId);
                    Result<decimal> conversionResult = await converterService.ConvertAsync(
                        sessionItem.Quantity,
                        sessionItem.UnitId,
                        foodItem.UnitId,
                        cancellationToken);
                    if (conversionResult.IsFailure)
                    {
                        throw new PentoException(nameof(TradeSessionCompletedEventHandler), conversionResult.Error);
                    }
                    foodItem.Reserve(conversionResult.Value, sessionItem.Quantity, sessionItem.UnitId, offer.UserId); //reserve again in case restored above
                    offeredItem.Update(sessionItem.Quantity, sessionItem.UnitId);
                    tradeItemOfferRepository.Update(offeredItem);
                } 
                else
                {
                    offeredItem = TradeItemOffer.Create(
                        sessionItem.FoodItemId,
                        sessionItem.Quantity,
                        sessionItem.UnitId,
                        offer.Id);
                    tradeItemOfferRepository.Add(offeredItem); //no need to reserve since already did when first added
                }
            }
            else if (sessionItem.From == TradeItemFrom.Request)
            {
                TradeItemRequest? requestedItem = requestedItems.FirstOrDefault(ri => ri.FoodItemId == sessionItem.FoodItemId);
                if (requestedItem != null)
                {
                    FoodItem foodItem = allInvolvedFoodItems.Single(fi => fi.Id == requestedItem.FoodItemId);
                    Result<decimal> conversionResult = await converterService.ConvertAsync(
                        sessionItem.Quantity,
                        sessionItem.UnitId,
                        foodItem.UnitId,
                        cancellationToken);
                    if (conversionResult.IsFailure)
                    {
                        throw new PentoException(nameof(TradeSessionCompletedEventHandler), conversionResult.Error);
                    }
                    foodItem.Reserve(conversionResult.Value, sessionItem.Quantity, sessionItem.UnitId, offer.UserId);
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
                    tradeItemRequestRepository.Add(requestedItem); //no need to reserve since already did when first added
                }
            } 
        }

        IEnumerable<FoodItem> foodItemsFromRequestToOffer = await foodItemRepository.FindAsync(
            fi => sessionItems.Where(s => s.From == TradeItemFrom.Request).Select(ri => ri.FoodItemId).Contains(fi.Id),
            cancellationToken);
        IEnumerable<FoodItem> foodItemsFromOfferToRequest = await foodItemRepository.FindAsync(
            fi => sessionItems.Where(s => s.From == TradeItemFrom.Offer).Select(oi => oi.FoodItemId).Contains(fi.Id),
            cancellationToken);
        Compartment offerCompartment = (await compartmentRepository.FindAsync(
            c => c.HouseholdId == offer.HouseholdId,
            cancellationToken)).First();
        Compartment requestCompartment = (await compartmentRepository.FindAsync(
            c => c.HouseholdId == request.HouseholdId,
            cancellationToken)).First();
        foreach (TradeSessionItem sessionItem in sessionItems)
        {
            if (sessionItem.From == TradeItemFrom.Offer)
            {
                FoodItem? foodItem = foodItemsFromOfferToRequest.FirstOrDefault(fi => fi.Id == sessionItem.FoodItemId);
                if (foodItem != null)
                {
                    FoodItem tradedItem = foodItem.Trade(request.HouseholdId, request.UserId, requestCompartment.Id, sessionItem.Quantity, sessionItem.UnitId, offer.UserId);
                    foodItemRepository.Add(tradedItem);
                    foodItemRepository.Update(foodItem);
                }
            }
            else if (sessionItem.From == TradeItemFrom.Request)
            {
                FoodItem? foodItem = foodItemsFromRequestToOffer.FirstOrDefault(fi => fi.Id == sessionItem.FoodItemId);
                if (foodItem != null)
                {
                    FoodItem tradedItem = foodItem.Trade(offer.HouseholdId, offer.UserId, offerCompartment.Id, sessionItem.Quantity, sessionItem.UnitId, offer.UserId);
                    foodItemRepository.Add(tradedItem);
                    foodItemRepository.Update(foodItem);
                }
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
