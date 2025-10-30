using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.Create;
using Pento.Application.Households.GenerateInvite;
using Pento.Application.Households.Join;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Households.Post;

internal sealed class JoinHousehold : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/households/join", async (
            IUserContext userContext,
            Request request,
            ICommandHandler<JoinHouseholdCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new JoinHouseholdCommand(userContext.UserId, request.InviteCode), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization();
    }
    internal sealed class Request
    {
        public string InviteCode { get; init; }
    }
}
