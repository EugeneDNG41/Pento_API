using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Offers.AdminGetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetTradeOfferById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/trades/offers/{tradeOfferId:guid}", async (
            Guid tradeOfferId,
            IQueryHandler<GetAdminTradeOfferByIdQuery, TradeOfferDetailAdminResponse> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetAdminTradeOfferByIdQuery(tradeOfferId);
            Result<TradeOfferDetailAdminResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .RequireAuthorization(Permissions.ManageUsers)
        .WithTags(Tags.Admin);
    }
}
