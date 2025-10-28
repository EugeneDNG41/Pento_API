﻿using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Units.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.Units;

internal sealed class GetUnit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("units/{unitId:guid}", async (Guid unitId, IQueryHandler<GetUnitQuery, UnitResponse>  handler, CancellationToken cancellationToken) =>
        {
            Result<UnitResponse> result = await handler.Handle(new GetUnitQuery(unitId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Units);
    }
}
