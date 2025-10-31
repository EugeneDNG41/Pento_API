
using Microsoft.AspNetCore.Mvc;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.GenerateInvite;
using Pento.Application.Households.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Households.Put;

internal sealed class UpdateHousehold : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("households", async (
            IUserContext userContext,
            Request request,
            ICommandHandler<UpdateHouseholdCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new UpdateHouseholdCommand(userContext.HouseholdId, request.Name), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization(policy => policy.RequireRole(Role.HouseholdHead.Name, Role.PowerMember.Name));
    }
    internal sealed class Request
    {
        public string Name { get; init; }
    }
}
