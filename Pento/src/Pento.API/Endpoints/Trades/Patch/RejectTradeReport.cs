using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Reports.Reject;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Patch;

internal sealed class RejectTradeReport : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("trades/reports/{tradeReportId:guid}/reject", async (
            Guid tradeReportId,
            ICommandHandler<RejectTradeReportCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RejectTradeReportCommand(tradeReportId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(
                () => Results.Ok(),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
    }
}
