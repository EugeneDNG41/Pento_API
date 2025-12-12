using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Requests.Reject;

internal sealed class RejectTradeRequestCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<RejectTradeRequestCommand>
{
    public async Task<Result> Handle(RejectTradeRequestCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        TradeRequest? tradeRequest = await tradeRequestRepository.GetByIdAsync(command.RequestId,cancellationToken);
        if (tradeRequest == null)
        {
            return Result.Failure(TradeErrors.RequestNotFound);
        }
        TradeOffer? tradeOffer =  await tradeOfferRepository.GetByIdAsync(tradeRequest.TradeOfferId,cancellationToken);
        if (tradeOffer == null)
        {
            return Result.Failure(TradeErrors.OfferNotFound);
        }
        if (tradeOffer.HouseholdId != householdId)
        {
            return Result.Failure(TradeErrors.OfferForbiddenAccess);
        }
        if (tradeRequest.Status != TradeRequestStatus.Pending)
        {
            return Result.Failure(TradeErrors.InvalidRequestState);
        }
        tradeRequest.Reject();
        tradeRequestRepository.Update(tradeRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
