using Pento.Domain.Users;
using Pento.API.Extensions;
using Pento.Application.PossibleUnits.Get;
using Pento.Domain.Abstractions;
using Pento.Infrastructure.Authentication;
using Pento.Application.Abstractions.Messaging;
namespace Pento.API.Endpoints.PossibleUnits;

internal sealed class GetPossibleUnits : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("possible-units", async (Guid foodRefId, IQueryHandler<GetPossibleUnitsQuery, IReadOnlyList<PossibleUnitResponse>> handler, CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<PossibleUnitResponse>> result = await handler.Handle(new GetPossibleUnitsQuery(foodRefId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("PossibleUnits");
    }
}
