using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades.Reports;

namespace Pento.Application.Trades.Reports.Resolve;

internal sealed class ResolveTradeReportCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<TradeReport> reportRepo,
    IUnitOfWork uow
) : ICommandHandler<ResolveTradeReportCommand>
{
    public async Task<Result> Handle(
        ResolveTradeReportCommand command,
        CancellationToken cancellationToken)
    {

        TradeReport? report =
            await reportRepo.GetByIdAsync(command.TradeReportId, cancellationToken);

        if (report is null)
        {
            return Result.Failure(TradeReportErrors.NotFound);
        }

        if (report.Status is TradeReportStatus.Resolved
            or TradeReportStatus.Rejected)
        {
            return Result.Failure(TradeReportErrors.InvalidStatus);
        }

        report.Resolve(dateTimeProvider.UtcNow);
        await reportRepo.UpdateAsync(report, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
