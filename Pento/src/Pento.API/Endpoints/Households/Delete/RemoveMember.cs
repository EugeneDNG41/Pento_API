﻿using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.Leave;
using Pento.Application.Households.RemoveMember;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Households.Delete;

internal sealed class RemoveMember : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("households/members/{userId:guid}", async (
            Guid userId,
            IUserContext userContext,
            ICommandHandler<RemoveHouseholdMemberCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new RemoveHouseholdMemberCommand(userContext.HouseholdId, userId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization(policy => policy.RequireRole(Role.HouseholdHead.Name, Role.PowerMember.Name));
    }
}
