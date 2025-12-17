using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Trades.Reports.CreateReportMedias;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;
using Pento.Domain.Trades.Reports;

namespace Pento.Application.Trades.Reports.Media.Create;

internal sealed class AddTradeReportMediaCommandHandler(
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IBlobService blobService,
    IGenericRepository<TradeReport> reportRepo,
    IGenericRepository<TradeReportMedia> mediaRepo,
    IUnitOfWork uow
) : ICommandHandler<AddTradeReportMediaCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        AddTradeReportMediaCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        TradeReport? report =
            await reportRepo.GetByIdAsync(command.TradeReportId, cancellationToken);

        if (report is null)
        {
            return Result.Failure<Guid>(TradeReportMediaErrors.ReportNotFound);
        }

        if (report.ReporterUserId != userId)
        {
            return Result.Failure<Guid>(TradeReportMediaErrors.Forbidden);
        }

        Result<Uri> uploadResult = command.MediaType switch
        {
            TradeReportMedia.TradeReportMediaType.Image =>
                await blobService.UploadImageAsync(
                    command.File,
                    domain: "trade-report",
                    cancellationToken: cancellationToken),

            TradeReportMedia.TradeReportMediaType.Video =>
                await blobService.UploadVideoAsync(
                    command.File,
                    domain: "trade-report",
                    cancellationToken: cancellationToken),

            _ => Result.Failure<Uri>(TradeReportMediaErrors.UploadFailed)
        };

        if (uploadResult.IsFailure)
        {
            return Result.Failure<Guid>(uploadResult.Error);
        }

        var media = TradeReportMedia.Create(
            tradeReportId: report.Id,
            mediaType: command.MediaType,
            mediaUri: uploadResult.Value,
            createdOn: dateTimeProvider.UtcNow
        );

        mediaRepo.Add(media);
        await uow.SaveChangesAsync(cancellationToken);

        return media.Id;
    }
}
