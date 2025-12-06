using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Storages.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Storages;

namespace Pento.API.Endpoints.Storages.Get;

internal sealed class GetStorages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("storages", async (
            string? searchText,
            StorageType? storageType,
            IQueryHandler<GetStoragesQuery, PagedList<StoragePreview>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<StoragePreview>> result = await handler.Handle(
                new GetStoragesQuery(searchText, storageType, pageNumber, pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Storages)
        .RequireAuthorization();
    }
}
