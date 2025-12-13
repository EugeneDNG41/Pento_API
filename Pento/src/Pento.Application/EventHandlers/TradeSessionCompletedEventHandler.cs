using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.Notifications;
using Pento.Domain.Trades;

namespace Pento.Application.EventHandlers;

internal sealed class TradeSessionCompletedEventHandler(
    IConverterService converterService,
    INotificationService notificationService,
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
        var offeredItems = (await tradeItemOfferRepository.FindAsync(
            tio => tio.OfferId == offer.Id,
            cancellationToken)).ToList();
        var requestedItems = (await tradeItemRequestRepository.FindAsync(
            tir => tir.RequestId == request.Id,
            cancellationToken)).ToList();
        var sessionRequestItems = (await tradeItemSessionRepository.FindAsync(
            tis => tis.SessionId == session.Id && tis.From == TradeItemFrom.Request,
            cancellationToken)).ToList();
        var sessionOfferItems = (await tradeItemSessionRepository.FindAsync(
            tis => tis.SessionId == session.Id && tis.From == TradeItemFrom.Offer,
            cancellationToken)).ToList();
        var allInvolvedFoodItems = (await foodItemRepository.FindAsync(
            fi => offeredItems.Select(oi => oi.FoodItemId).Contains(fi.Id) ||
                  requestedItems.Select(ri => ri.FoodItemId).Contains(fi.Id) ||
                  sessionRequestItems.Select(si => si.FoodItemId).Contains(fi.Id) ||
                  sessionOfferItems.Select(si => si.FoodItemId).Contains(fi.Id),
            cancellationToken)).ToList();
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
            foodItem.AdjustReservedQuantity(conversionResult.Value); //restore reserved quantity
            bool inSession = sessionOfferItems.Any(si => si.FoodItemId == offeredItem.FoodItemId);
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
            foodItem.AdjustReservedQuantity(conversionResult.Value); //restore reserved quantity
            bool inSession = sessionRequestItems.Any(si => si.From == TradeItemFrom.Request && si.FoodItemId == requestedItem.FoodItemId);
            if (!inSession)
            {
                tradeItemRequestRepository.Remove(requestedItem);
            }
        }

        foreach (TradeSessionItem sessionItem in sessionOfferItems) //overwrite offered/requested quantities with actual traded quantities
        {
            TradeItemOffer? offeredItem = offeredItems.SingleOrDefault(oi => oi.FoodItemId == sessionItem.FoodItemId);
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
        foreach (TradeSessionItem sessionItem in sessionRequestItems) //overwrite offered/requested quantities with actual traded quantities
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
       var foodItemsFromRequestToOffer = (await foodItemRepository.FindAsync(
            fi => sessionRequestItems.Select(ri => ri.FoodItemId).Contains(fi.Id),
            cancellationToken)).ToList();
        var foodItemsFromOfferToRequest = (await foodItemRepository.FindAsync(
            fi => sessionOfferItems.Select(oi => oi.FoodItemId).Contains(fi.Id),
            cancellationToken)).ToList();
        Compartment offerCompartment = (await compartmentRepository.FindAsync(
            c => c.HouseholdId == offer.HouseholdId,
            cancellationToken)).First();
        Compartment requestCompartment = (await compartmentRepository.FindAsync(
            c => c.HouseholdId == request.HouseholdId,
            cancellationToken)).First();
        foreach (TradeSessionItem sessionItem in sessionOfferItems)
        {
            FoodItem? foodItem = foodItemsFromOfferToRequest.FirstOrDefault(fi => fi.Id == sessionItem.FoodItemId);
            if (foodItem != null)
            {
                FoodItem tradedItem = foodItem.Trade(request.HouseholdId, request.UserId, requestCompartment.Id, sessionItem.Quantity, sessionItem.UnitId, offer.UserId);
                foodItemRepository.Add(tradedItem);
                foodItemRepository.Update(foodItem);
            }
        }
        foreach (TradeSessionItem sessionItem in sessionRequestItems)
        { 
            FoodItem? foodItem = foodItemsFromRequestToOffer.FirstOrDefault(fi => fi.Id == sessionItem.FoodItemId);
            if (foodItem != null)
            {
                FoodItem tradedItem = foodItem.Trade(offer.HouseholdId, offer.UserId, offerCompartment.Id, sessionItem.Quantity, sessionItem.UnitId, offer.UserId);
                foodItemRepository.Add(tradedItem);
                foodItemRepository.Update(foodItem);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);

        string title = "Trade Completed";
        string requestItemsToOfferMessage = sessionRequestItems.Any()
            ? $"You received {sessionRequestItems.Count} item(s) in {offerCompartment.Name} from the trade."
            : "No items were received from the trade.";
        string offerItemsToRequestMessage = sessionOfferItems.Any()
            ? $"You received {sessionOfferItems.Count} item(s) in {requestCompartment.Name} from the trade."
            : "No items were sent from the trade.";
        var payload = new Dictionary<string, string>
        {
            { "TradeSessionId", session.Id.ToString() },
            { "CompartmentId", offerCompartment.Id.ToString() }
        };
        Result receiveRequestedItemsNotification = await notificationService.SendToHouseholdAsync(
            offer.HouseholdId,
            title,
            requestItemsToOfferMessage,
            NotificationType.Trade,
            payload,
            cancellationToken);
        payload.Remove("CompartmentId");
        payload.Add("CompartmentId", requestCompartment.Id.ToString());
        Result receiveOfferedItemsNotification = await notificationService.SendToHouseholdAsync(
            request.HouseholdId,
            title,
            offerItemsToRequestMessage,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (receiveRequestedItemsNotification.IsFailure) 
        {
            throw new PentoException(nameof(TradeSessionCompletedEventHandler), receiveRequestedItemsNotification.Error);
        }
        if (receiveOfferedItemsNotification.IsFailure) 
        {
            throw new PentoException(nameof(TradeSessionCompletedEventHandler), receiveOfferedItemsNotification.Error);
        }
    }
}
