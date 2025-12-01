using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Features.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Features.Get;

internal sealed class GetFeatures : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("features", async (
            string? searchText,
            IQueryHandler<GetFeaturesQuery, IReadOnlyList<FeatureResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<FeatureResponse>> result = await handler.Handle(new GetFeaturesQuery(searchText), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization(Permissions.ManageSubscriptions).WithTags(Tags.Admin);
    }
}
