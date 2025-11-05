using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Storages.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Storages.Put;

internal sealed class UpdateStorage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("storages/{storageId:guid}", async (
            Guid storageId,
            Request request,
            ICommandHandler<UpdateStorageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new UpdateStorageCommand(storageId, request.Name, request.Notes), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Storages)
        .RequireAuthorization();
    }
    internal sealed class Request
    {
        public string Name { get; init; }
        public string? Notes { get; init; }
    }

}
