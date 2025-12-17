using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Requests.Cancel;

internal sealed class CancelTradeRequestCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<CancelTradeRequestCommand>
{
    public async Task<Result> Handle(CancelTradeRequestCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        TradeRequest? tradeRequest = await tradeRequestRepository.GetByIdAsync(command.RequestId,cancellationToken);
        if (tradeRequest == null)
        {
            return Result.Failure(TradeErrors.RequestNotFound);
        }
        if (tradeRequest.HouseholdId != householdId)
        {
            return Result.Failure(TradeErrors.RequestForbiddenAccess);
        }
        if (tradeRequest.Status != TradeRequestStatus.Pending)
        {
            return Result.Failure(TradeErrors.InvalidRequestState);
        }
        IEnumerable<TradeSession> ongoingSessions = await tradeSessionRepository.FindAsync(
            ts => ts.TradeRequestId == tradeRequest.Id && ts.Status == TradeSessionStatus.Ongoing,
            cancellationToken);
        foreach (TradeSession session in ongoingSessions)
        {
            session.Cancel();
            await tradeSessionRepository.UpdateAsync(session, cancellationToken);
        }
        tradeRequest.Cancel();     
        await tradeRequestRepository.UpdateAsync(tradeRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
