using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Offers.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Delete;

internal sealed class DeleteTradeOffer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("trades/offers/{tradeOfferId:guid}", async (
            Guid tradeOfferId,
            ICommandHandler<DeleteTradeOfferCommand> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new DeleteTradeOfferCommand(tradeOfferId);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(
                Results.NoContent,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
    }
}
