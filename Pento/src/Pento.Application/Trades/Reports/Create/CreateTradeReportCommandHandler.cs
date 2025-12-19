using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Trades.Reports;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Reports.Create;

internal sealed class CreateTradeReportCommandHandler(
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<TradeSession> sessionRepo,
    IGenericRepository<TradeOffer> offerRepo,
    IGenericRepository<TradeRequest> requestRepo,
    IGenericRepository<TradeReport> reportRepo,
    IUnitOfWork uow
) : ICommandHandler<CreateTradeReportCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateTradeReportCommand command,
        CancellationToken cancellationToken)
    {
        Guid reporterUserId = userContext.UserId;
        Guid? householdId = userContext.HouseholdId;

        if (reporterUserId == Guid.Empty)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }

        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        if (reporterUserId == command.ReportedUserId)
        {
            return Result.Failure<Guid>(TradeReportErrors.CannotReportYourself);
        }

        TradeSession? session =
            await sessionRepo.GetByIdAsync(command.TradeSessionId, cancellationToken);

        if (session is null)
        {
            return Result.Failure<Guid>(TradeReportErrors.TradeSessionNotFound);
        }

        if (session.Status != TradeSessionStatus.Completed || session.Status != TradeSessionStatus.Cancelled )
        {
            return Result.Failure<Guid>(TradeReportErrors.TradeNotCompletedOrCancel);
        }

        TradeOffer? offer =
            await offerRepo.GetByIdAsync(session.TradeOfferId, cancellationToken);

        if (offer is null)
        {
            return Result.Failure<Guid>(TradeErrors.OfferNotFound);
        }

        TradeRequest? request =
            await requestRepo.GetByIdAsync(session.TradeRequestId, cancellationToken);

        if (request is null)
        {
            return Result.Failure<Guid>(TradeErrors.RequestNotFound);
        }

        Guid[] validUserIds = { offer.UserId, request.UserId };

        if (!validUserIds.Contains(reporterUserId) ||
            !validUserIds.Contains(command.ReportedUserId))
        {
            return Result.Failure<Guid>(TradeReportErrors.InvalidParticipants);
        }

        bool alreadyReported = await reportRepo.AnyAsync(
            r => r.TradeSessionId == command.TradeSessionId
              && r.ReporterUserId == reporterUserId,
            cancellationToken);

        if (alreadyReported)
        {
            return Result.Failure<Guid>(TradeReportErrors.DuplicateReport);
        }

        var report = TradeReport.Create(
            tradeSessionId: command.TradeSessionId,
            reporterUserId: reporterUserId,
            reportedUserId: command.ReportedUserId,
            reason: command.Reason,
            severity: command.Severity,
            description: command.Description,
            createdOn: dateTimeProvider.UtcNow
        );

        reportRepo.Add(report);
        await uow.SaveChangesAsync(cancellationToken);

        return report.Id;
    }
}
