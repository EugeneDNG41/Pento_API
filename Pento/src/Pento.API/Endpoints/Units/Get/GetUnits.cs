using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Units.Get;
using Pento.Application.Units.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Units.Get;

internal sealed class GetUnits : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("units", async (
            IQueryHandler<GetUnitsQuery, IReadOnlyList<UnitResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<UnitResponse>> result = await handler.Handle(
                new GetUnitsQuery(), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Units);
    }
}
