using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Units.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Units;

namespace Pento.API.Endpoints.Admin.Post;

internal sealed class CreateUnit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("admin/units", async (Request request, ICommandHandler<CreateUnitCommand, Guid> handler, CancellationToken cancellationToken) =>
        {
            var command = new CreateUnitCommand(
                request.Name,
                request.Abbreviation,
                request.ToBaseFactor,
                request.Type
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/units/{id}", new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Admin);
    }

    internal sealed class Request
    {
        public string Name { get; init; } = string.Empty;
        public string Abbreviation { get; init; } = string.Empty;
        public decimal ToBaseFactor { get; init; }
        public UnitType Type { get; init; }
    }
}
