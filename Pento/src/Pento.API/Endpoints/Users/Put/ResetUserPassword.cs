
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.ResetPassword;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Put;

internal sealed class ResetUserPassword : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/password/reset", async (
            Request request,
            ICommandHandler<ResetUserPasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new ResetUserPasswordCommand(request.Email), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Users).AllowAnonymous();
    }
    internal sealed class Request
    {
        public string Email { get; init; } 
    }
}
