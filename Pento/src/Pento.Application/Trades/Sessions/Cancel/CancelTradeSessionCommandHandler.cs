using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.Cancel;

internal sealed class CancelTradeSessionCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<CancelTradeSessionCommand>
{
    public async Task<Result> Handle(CancelTradeSessionCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId == null)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(command.TradeSessionId, cancellationToken);
        if (session == null)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.SessionNotFound);
        }
        if (session.OfferHouseholdId != householdId && session.RequestHouseholdId != householdId)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.InvalidSessionState);
        }     
        session.Cancel();
        await tradeSessionRepository.UpdateAsync(session, cancellationToken);
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(session.TradeRequestId, cancellationToken);
        if (request == null)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.RequestNotFound);
        }
        if (request.Status != TradeRequestStatus.Pending)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.InvalidRequestState);
        }
        if (request.HouseholdId == householdId)
        {
            request.Cancel();
        } 
        else
        {
            request.Reject();
        }
        await tradeRequestRepository.UpdateAsync(request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.Group(session.Id.ToString())
            .TradeSessionCancelled(session.Id);
        return Result.Success();
    }
}
