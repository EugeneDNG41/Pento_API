using MediatR;
using Pento.API.Extensions;
using Pento.Application.Units.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.Units;

internal sealed class CreateUnit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("units", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateUnitCommand(
                request.Name,
                request.Abbreviation,
                request.ToBaseFactor
            );

            Result<Guid> result = await sender.Send(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/units/{id}", new { Id = id }),
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.Units);
    }

    internal sealed class Request
    {
        public string Name { get; init; } = string.Empty;
        public string Abbreviation { get; init; } = string.Empty;
        public decimal ToBaseFactor { get; init; }
    }
}
