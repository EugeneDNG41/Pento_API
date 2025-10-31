using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.SetRoles;
using Pento.Application.Households.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Households.Put;

internal sealed class SetMemberRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("households/members/{memberId:guid}/roles", async (
            Guid memberId,
            IUserContext userContext,
            Request request,
            ICommandHandler<SetMemberRolesCommand> handler,
            CancellationToken cancellationToken) =>
        {
            SetMemberRolesCommand command = new(userContext.HouseholdId, memberId, request.Roles);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization(policy => policy.RequireRole(Role.HouseholdHead.Name));
    }
    internal sealed class Request
    {
        public List<string> Roles { get; init; }
    }
}

