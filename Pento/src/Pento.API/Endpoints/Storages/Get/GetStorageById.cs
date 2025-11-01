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
            IQueryHandler<GetStorageByIdQuery, StorageResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<StorageResponse> result = await handler.Handle(
                new GetStorageByIdQuery(storageId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Storages)
        .RequireAuthorization()
        .WithName(RouteNames.GetStorageById);
    }
}
