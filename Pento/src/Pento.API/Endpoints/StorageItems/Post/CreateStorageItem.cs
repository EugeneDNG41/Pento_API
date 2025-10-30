using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.StorageItems.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.StorageItems.Post;

internal sealed class CreateStorageItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("storage-items", async (
            Request request, 
            ICommandHandler<CreateStorageItemCommand, Guid> handler, 
            CancellationToken cancellationToken) =>
        {
            var command = new CreateStorageItemCommand(
                request.FoodRefId,
                request.CompartmentId,
                request.CustomName,
                request.Quantity,
                request.UnitId,
                request.ExpirationDate.ToUniversalTime(),
                request.Notes);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                guid => Results.CreatedAtRoute(
                    routeName: "GetStorageItemById",
                    routeValues: new { id = guid },      
                    value: new { id = guid }),
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.StorageItems);
    }
    internal sealed class Request
    {
        public Guid FoodRefId { get; init; }
        public Guid CompartmentId { get; init; }
        public string? CustomName { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
        public DateTime ExpirationDate{ get; init; }
        public string? Notes { get; init; }
    }
}
