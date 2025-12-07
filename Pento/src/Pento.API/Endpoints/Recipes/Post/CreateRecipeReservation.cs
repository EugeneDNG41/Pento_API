using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Reserve;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Post;

internal sealed class CreateRecipeReservation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipes/reserve", async (
      CreateRecipeReservationCommand request,
      ICommandHandler<CreateRecipeReservationCommand, Guid> handler,
      CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(request, cancellationToken);

            return result.Match(
                id => Results.Ok(new { id }),
                CustomResults.Problem);
        })
  .WithTags(Tags.Reservations);
    }
}
