using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Storages.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;
using Pento.Domain.Storages;

namespace Pento.API.Endpoints.Storages.Post;

internal sealed class CreateStorage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("storages", async (
            Request request,
            ICommandHandler<CreateStorageCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(
                new CreateStorageCommand(request.Name, request.Type, request.Notes), cancellationToken);
            return result
            .Match(id => Results.CreatedAtRoute(RouteNames.GetStorageById, new { storageId = id }, id), CustomResults.Problem);
        })
        .WithTags(Tags.Storages)
        .RequireAuthorization(policy => policy.RequireRole(Role.HouseholdHead.Name, Role.PowerMember.Name, Role.PantryManager.Name));
    }
    internal sealed class Request
    {
        public string Name { get; init; }
        public StorageType Type { get; init; }
        public string? Notes { get; init; }
    }
}
