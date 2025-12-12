using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.Cancel;

internal sealed class CancelTradeOfferCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<CancelTradeOfferCommand>
{
    public async Task<Result> Handle(CancelTradeOfferCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        TradeOffer? tradeOffer = await tradeOfferRepository.GetByIdAsync(command.OfferId,cancellationToken);
        if (tradeOffer == null)
        {
            return Result.Failure(TradeErrors.OfferNotFound);
        }
        if (tradeOffer.HouseholdId != householdId)
        {
            return Result.Failure(TradeErrors.OfferForbiddenAccess);
        }
        if (tradeOffer.Status != TradeOfferStatus.Open)
        {
            return Result.Failure(TradeErrors.InvalidOfferState);
        }
        tradeOffer.Cancel();
        tradeOfferRepository.Update(tradeOffer);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
