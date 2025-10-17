using MediatR;
using Pento.API.Extensions;
using Pento.Application.Units.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.Units;

internal sealed class GetUnit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("units/{unitId:guid}", async (Guid unitId, ISender sender, CancellationToken cancellationToken) =>
        {
            Result<UnitResponse> result = await sender.Send(new GetUnitQuery(unitId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Units);
    }
}
