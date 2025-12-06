using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
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
            string? searchText,
            IQueryHandler<GetCompartmentsQuery, PagedList<CompartmentPreview>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<CompartmentPreview>> result = await handler.Handle(
                new GetCompartmentsQuery(storageId, searchText, pageNumber, pageSize), cancellationToken);
            return result
            .Match(compartments => Results.Ok(compartments), CustomResults.Problem);
        })
        .WithTags(Tags.Compartments)
        .RequireAuthorization();
    }
}
