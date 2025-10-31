
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.GetRoles;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Households.Get;

internal sealed class GetHouseholdRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("households/roles", async (
            IQueryHandler<GetHouseholdRolesQuery, IReadOnlyList<RoleResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetHouseholdRolesQuery();
            Result<IReadOnlyList<RoleResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                roles => Results.Ok(roles),
                CustomResults.Problem
            );
        }).RequireAuthorization().WithTags(Tags.Households);
    }
}
