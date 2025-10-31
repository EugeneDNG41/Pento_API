using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.API.Endpoints.FoodItems.Post;

internal sealed class CreateFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-items", async (
            IUserContext userContext,
            Request request, 
            ICommandHandler<CreateFoodItemCommand, Guid> handler, 
            CancellationToken cancellationToken) =>
        {

            var command = new CreateFoodItemCommand(
                request.FoodRefId,
                request.CompartmentId,
                userContext.HouseholdId,
                request.CustomName,
                request.Quantity,
                request.UnitId,
                request.ExpirationDate,
                request.Notes);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                guid => Results.CreatedAtRoute(
                    routeName: "GetFoodItemById",
                    routeValues: new { id = guid },      
                    value: new { id = guid }),
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.FoodItems);
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
