
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Subscriptions.Get;

internal sealed class GetSubscriptionById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("subscriptions/{subscriptionId:guid}", async (
            Guid subscriptionId,
            IQueryHandler<GetSubscriptionByIdQuery, SubscriptionDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<SubscriptionDetailResponse> result = await handler.Handle(new GetSubscriptionByIdQuery(subscriptionId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Subscriptions).WithName(RouteNames.GetSubscriptionById);
    }
}
