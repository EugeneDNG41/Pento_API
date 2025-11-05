using Microsoft.Identity.Client.Extensions.Msal;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Storages.Delete;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Storages.Delete;

internal sealed class DeleteStorage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("storages/{storageId:guid}", async (
            Guid storageId,
            ICommandHandler<DeleteStorageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new DeleteStorageCommand(storageId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Storages)
        .RequireAuthorization();
    }
}
