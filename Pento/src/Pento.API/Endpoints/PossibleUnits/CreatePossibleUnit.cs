using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.PossibleUnits.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.PossibleUnits;

internal sealed class CreatePossibleUnit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("possible-units", async (Request request, ICommandHandler<CreatePossibleUnitCommand, Guid> handler, CancellationToken cancellationToken) =>
        {
            var command = new CreatePossibleUnitCommand(
                request.UnitId,
                request.FoodRefId,
                request.IsDefault
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/possible-units/{id}", new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags("PossibleUnits");
    }

    internal sealed class Request
    {
        public Guid UnitId { get; init; }
        public Guid FoodRefId { get; init; }
        public bool IsDefault { get; init; }
    }
}
