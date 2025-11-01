using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Storages.GetAll;
using Pento.Application.Storages.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Storages.Get;

internal sealed class GetStorages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("storages", async (
            IQueryHandler<GetStoragesQuery, IReadOnlyList<StorageResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<StorageResponse>> result = await handler.Handle(
                new GetStoragesQuery(), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Storages)
        .RequireAuthorization();
    }
}
