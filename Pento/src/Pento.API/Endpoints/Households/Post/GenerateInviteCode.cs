using Microsoft.AspNetCore.Authorization;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.Create;
using Pento.Application.Households.GenerateInvite;
using Pento.Application.Households.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.Households.Post;

internal sealed class GenerateInviteCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("households/invites", async (
            IUserContext userContext,
            Request request,
            ICommandHandler<GenerateInviteCodeCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            Result<string> result = await handler.Handle(
                new GenerateInviteCodeCommand(userContext.HouseholdId, request.CodeExpirationDate), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization(policy => policy.RequireRole(Role.HouseholdHead.Name, Role.PowerMember.Name));
    }
    internal sealed class Request
    {
        public DateTime? CodeExpirationDate { get; init; }
    }
}
