using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.Create;
using Pento.Application.Households.GenerateInvite;
using Pento.Application.Households.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.API.Endpoints.Households.Post;

internal sealed class GenerateInviteCode : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/households/{householdId:Guid}/invites", async (
            Guid householdId,
            IUserContext userContext,
            Request request,
            ICommandHandler<GenerateInviteCodeCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            DateTime? expiresAtUtc = request.CodeExpirationDate?.ToUniversalTime();
            Result<string> result = userContext.HouseholdId != householdId
                ? Result.Failure<string>(HouseholdErrors.NotFound)
                : await handler.Handle(
                new GenerateInviteCodeCommand(householdId, expiresAtUtc), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization(Role.HouseholdAdmin.Name, Role.PowerMember.Name);
    }
    internal sealed class Request
    {
        public DateTime? CodeExpirationDate { get; init; }
    }
}
