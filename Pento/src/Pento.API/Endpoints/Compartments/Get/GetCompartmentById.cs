using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Compartments.Get;

internal sealed class GetCompartmentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("compartments/{compartmentId:guid}", async (
            Guid compartmentId,
            IQueryHandler<GetCompartmentByIdQuery, CompartmentResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<CompartmentResponse> result = await handler.Handle(
                new GetCompartmentByIdQuery(compartmentId), cancellationToken);
            return result
            .Match(compartment => Results.Ok(compartment), CustomResults.Problem);
        })
        .WithTags(Tags.Compartments)
        .RequireAuthorization(policy => policy.RequireRole(Role.HouseholdHead.Name, Role.PowerMember.Name, Role.PantryManager.Name, Role.ErrandRunner.Name))
        .WithName(RouteNames.GetCompartmentById);
    }
}
