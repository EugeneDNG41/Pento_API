using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Storages.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Storages.Get;

internal sealed class GetStorageById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("storages/{storageId:guid}", async (
            Guid storageId,
            IQueryHandler<GetStorageByIdQuery, StorageDetailResponse> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<StorageDetailResponse> result = await handler.Handle(
                new GetStorageByIdQuery(storageId, pageNumber, pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Storages)
        .RequireAuthorization()
        .WithName(RouteNames.GetStorageById);
    }
}
