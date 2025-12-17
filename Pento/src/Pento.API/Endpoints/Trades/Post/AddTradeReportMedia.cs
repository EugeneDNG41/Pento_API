using Microsoft.AspNetCore.Http;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Reports.CreateReportMedias;
using Pento.Application.Trades.Reports.Media.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.API.Endpoints.Trades.Post;

internal sealed class AddTradeReportMedia : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("trades/reports/{tradeReportId:guid}/media", async (
            Guid tradeReportId,
            IFormFile file,
            TradeReportMedia.TradeReportMediaType mediaType,
            ICommandHandler<AddTradeReportMediaCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddTradeReportMediaCommand(
                tradeReportId,
                mediaType,
                file
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization()
        .WithDescription("Media: images, videos")
        .DisableAntiforgery();
    }
}
