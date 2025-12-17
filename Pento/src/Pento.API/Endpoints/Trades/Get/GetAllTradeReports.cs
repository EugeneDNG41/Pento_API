using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Reports.Get;
using Pento.Application.Trades.Reports.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Get;

internal sealed class GetAllTradeReports : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("trades/reports", async (
            IQueryHandler<GetAllTradeReportsQuery, IReadOnlyList<TradeReportResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllTradeReportsQuery();

            Result<IReadOnlyList<TradeReportResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
        app.MapGet("trades/reports/{tradeReportId:guid}", async (
            Guid tradeReportId,
            IQueryHandler<GetTradeReportByIdQuery, IReadOnlyList<TradeReportResponse>> handler,
            CancellationToken cancellationToken) =>
                {
                    var query = new GetTradeReportByIdQuery(tradeReportId);

                    Result<IReadOnlyList<TradeReportResponse>> result = await handler.Handle(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        CustomResults.Problem
                    );
                })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
        app.MapGet("trades/reports/me", async (
            IQueryHandler<GetMyTradeReportsQuery, IReadOnlyList<TradeReportResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMyTradeReportsQuery();

            Result<IReadOnlyList<TradeReportResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
    }
}
