using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Get;
using Pento.Application.Compartments.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Compartments.Get;

internal sealed class GetCompartments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("storages/{storageId:guid}/compartments", async (
            Guid storageId,
            IQueryHandler<GetCompartmentsQuery, IReadOnlyList<CompartmentWithFoodItemPreviewResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<CompartmentWithFoodItemPreviewResponse>> result = await handler.Handle(
                new GetCompartmentsQuery(storageId), cancellationToken);
            return result
            .Match(compartments => Results.Ok(compartments), CustomResults.Problem);
        })
        .WithTags(Tags.Compartments)
        .RequireAuthorization();
    }
}
