using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Reports.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades.Reports;

namespace Pento.API.Endpoints.Trades.Post;

internal sealed class CreateTradeReport : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trades/reports", async (
            Request request,
            ICommandHandler<CreateTradeReportCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateTradeReportCommand(
                request.TradeSessionId,
                request.ReportedUserId,
                request.Reason,
                request.Severity,
                request.Description
            );
            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                 Results.Ok,
                 CustomResults.Problem
             );
        })
        .WithTags(Tags.Trades)
        .WithDescription("  TradeReportReason\r\n  {\r\n        FoodSafetyConcern,\r\n        ExpiredFood,\r\n        PoorHygiene,\r\n        MisleadingInformation,\r\n        InappropriateBehavior,\r\n        Other\r\n    }\r\n\r\n     FoodSafetyIssueLevel\r\n    {\r\n        Minor,\r\n        Serious,\r\n        Critical\r\n    }")
        .RequireAuthorization();
    }

    internal sealed class Request
    {
        public Guid TradeSessionId { get; init; }
        public Guid ReportedUserId { get; init; }
        public TradeReportReason Reason { get; init; }
        public FoodSafetyIssueLevel Severity { get; init; }
        public string? Description { get; init; }
    }
  
}
