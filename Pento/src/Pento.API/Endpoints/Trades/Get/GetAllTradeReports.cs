using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Reports.Get;
using Pento.Application.Trades.Reports.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades.Reports;

namespace Pento.API.Endpoints.Trades.Get;

internal sealed class GetAllTradeReports : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("trades/reports", async (
             TradeReportStatus? status,
             FoodSafetyIssueLevel? severity,
             TradeReportReason? reason,
             TradeReportSort sort,
             IQueryHandler<GetAllTradeReportsQuery, TradeReportPagedResponse> handler,
             CancellationToken cancellationToken,
                int pageNumber = 1,
             int pageSize = 5
             ) =>
        {
            var query = new GetAllTradeReportsQuery(
                PageNumber: pageNumber <= 0 ? 1 : pageNumber,
                PageSize: pageSize <= 0 ? 10 : pageSize,
                Status: status,
                Severity: severity,
                Reason: reason,
                Sort: sort
            );

            Result<TradeReportPagedResponse> result =
                await handler.Handle(query, cancellationToken);

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
