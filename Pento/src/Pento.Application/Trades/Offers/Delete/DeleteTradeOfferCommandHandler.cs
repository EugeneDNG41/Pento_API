using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.Delete;

internal sealed class DeleteTradeOfferCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<DeleteTradeOfferCommand>
{
    public async Task<Result> Handle(DeleteTradeOfferCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        TradeOffer? tradeOffer = await tradeOfferRepository.GetByIdAsync(command.OfferId, cancellationToken);
        if (tradeOffer == null)
        {
            return Result.Failure(TradeErrors.OfferNotFound);
        }
        if (tradeOffer.HouseholdId != householdId)
        {
            return Result.Failure(TradeErrors.OfferForbiddenAccess);
        }
        if (tradeOffer.Status == TradeOfferStatus.Open)
        {
            tradeOffer.Cancel();
        }
        IEnumerable<TradeSession> ongoingSessions = await tradeSessionRepository.FindAsync(
            ts => ts.TradeOfferId == tradeOffer.Id && ts.Status == TradeSessionStatus.Ongoing,
            cancellationToken);
        foreach (TradeSession session in ongoingSessions)
        {
            session.Cancel();
            await tradeSessionRepository.UpdateAsync(session, cancellationToken);
        }
        tradeOffer.Delete();
        await tradeOfferRepository.UpdateAsync(tradeOffer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
