using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Reports.Resolve;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Patch;

internal sealed class ResolveTradeReport : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("trades/reports/{tradeReportId:guid}/resolve", async (
            Guid tradeReportId,
            ICommandHandler<ResolveTradeReportCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ResolveTradeReportCommand(tradeReportId);

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
