using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.AdminGetById;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetTradeSessionById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/trades/sessions/{sessionId:guid}", async (
            Guid sessionId,
            IQueryHandler<GetAdminTradeSessionByIdQuery, TradeSessionDetailResponse> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetAdminTradeSessionByIdQuery(sessionId);
            Result<TradeSessionDetailResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        }).RequireAuthorization(Permissions.ManageUsers)
        .WithTags(Tags.Admin);
    }
}
