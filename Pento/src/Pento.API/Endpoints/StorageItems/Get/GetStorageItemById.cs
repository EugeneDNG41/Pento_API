using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.StorageItems.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.StorageItems.Get;

internal sealed class GetStorageItemById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("storage-items/{id:guid}", async (
            Guid id, 
            IQueryHandler<GetStorageItemQuery, StorageItemResponse> handler, 
            CancellationToken cancellationToken) =>
        {
            var query = new GetStorageItemQuery(id);

            Result<StorageItemResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                response => Results.Ok(response),
                CustomResults.Problem
            );
        })
        .WithName("GetStorageItemById")
        .RequireAuthorization()
        .WithTags(Tags.StorageItems);
    }
}
