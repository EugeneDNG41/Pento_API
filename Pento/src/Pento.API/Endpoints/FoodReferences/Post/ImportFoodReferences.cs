using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Import;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences.Post;

internal sealed class ImportFoodReferences : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/import", async (
            IFormFile file,
            ICommandHandler<ImportFoodReferencesCommand, int> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ImportFoodReferencesCommand(file);
            Result<int> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                count => Results.Ok(new
                {
                    Message = $"Imported {count} foundation_food records successfully.",
                    Total = count
                }),
                CustomResults.Problem
            );
        })
            .DisableAntiforgery()
        .WithTags(Tags.FoodReferences)
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
