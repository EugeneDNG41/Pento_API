
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.ChangePassword;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Put;

internal sealed class ChangeUserPassword : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/password", async (
            Request request,
            ICommandHandler<ChangeUserPasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new ChangeUserPasswordCommand(request.CurrentPassword, request.NewPassword, request.ConfirmNewPassword), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
    internal sealed class Request
    {
        public string CurrentPassword { get; init; }
        public string NewPassword { get; init; }
        public string ConfirmNewPassword { get; init; }
    }
}
