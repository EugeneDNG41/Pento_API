using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.DietaryTags.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.DietaryTags.Get;

internal sealed class GetAllDietaryTags : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dietary-tags", async (
            IQueryHandler<GetDietaryTagsQuery, IReadOnlyList<DietaryTagResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<DietaryTagResponse>> result =
                await handler.Handle(new GetDietaryTagsQuery(), cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.DietaryTags);
    }
}
