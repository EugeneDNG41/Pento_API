using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.DietaryTags.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.DietaryTags.Get;

internal sealed class GetDietaryTagById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dietary-tags/{id:guid}", async (
            Guid id,
            IQueryHandler<GetDietaryTagByIdQuery, DietaryTagResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<DietaryTagResponse> result =
                await handler.Handle(new GetDietaryTagByIdQuery(id), cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.DietaryTags);
    }
}
