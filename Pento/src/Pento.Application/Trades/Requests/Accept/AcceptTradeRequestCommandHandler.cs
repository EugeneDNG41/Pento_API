using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Requests.Accept;

internal sealed class AcceptTradeRequestCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeItemOffer> tradeItemOfferRepository,
    IGenericRepository<TradeItemRequest> tradeItemRequestRepository,
    IGenericRepository<TradeSessionItem> tradeItemSessionRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<AcceptTradeRequestCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AcceptTradeRequestCommand command, CancellationToken cancellationToken)
    {
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(command.OfferId, cancellationToken);
        if (offer == null)
        {
            return Result.Failure<Guid>(TradeErrors.OfferNotFound);
        }
        if (offer.UserId != userContext.UserId)
        {
            return Result.Failure<Guid>(TradeErrors.OfferForbiddenAccess);
        }
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            return Result.Failure<Guid>(TradeErrors.RequestNotFound);
        }
        if (offer.Status != TradeOfferStatus.Open)
        {
            return Result.Failure<Guid>(TradeErrors.InvalidOfferState);
        }
        if (request.Status != TradeRequestStatus.Pending)
        {
            return Result.Failure<Guid>(TradeErrors.InvalidRequestState);
        }
        if (offer.UserId == request.UserId)
        {
            return Result.Failure<Guid>(TradeErrors.CannotTradeWithSelf);
        }
        if (offer.HouseholdId == request.HouseholdId)
        {
            return Result.Failure<Guid>(TradeErrors.CannotTradeWithinHousehold);
        }
        var session = TradeSession.Create(offer.Id, request.Id, offer.UserId, request.UserId, dateTimeProvider.UtcNow);
        tradeSessionRepository.Add(session);
        IEnumerable<TradeItemOffer> offerItems = await tradeItemOfferRepository.FindAsync(
            item => item.OfferId == session.TradeOfferId,
            cancellationToken);
        IEnumerable<TradeItemRequest> requestItems = await tradeItemRequestRepository.FindAsync(
            item => item.RequestId == session.TradeRequestId,
            cancellationToken);
        var sessionItems = new List<TradeSessionItem>();
        foreach (TradeItemOffer offerItem in offerItems)
        {
            var sessionItem = TradeSessionItem.Create(
                offerItem.FoodItemId,
                offerItem.Quantity,
                offerItem.UnitId,
                TradeItemFrom.Offer,
                session.Id);
            sessionItems.Add(sessionItem);
        }
        foreach (TradeItemRequest requestItem in requestItems)
        {
            var sessionItem = TradeSessionItem.Create(
                requestItem.FoodItemId,
                requestItem.Quantity,
                requestItem.UnitId,
                TradeItemFrom.Request,
                session.Id);
            sessionItems.Add(sessionItem);
        }
        tradeItemSessionRepository.AddRange(sessionItems);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return session.Id;
    }
}
