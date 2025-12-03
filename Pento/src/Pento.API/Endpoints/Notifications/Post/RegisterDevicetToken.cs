using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Notifications.DeviceTokens;
using Pento.Domain.Abstractions;
using Pento.Domain.DeviceTokens;

namespace Pento.API.Endpoints.Notifications.Post;

internal sealed class RegisterDeviceTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("notifications/register-token", async (
            Request request,
            ICommandHandler<RegisterDeviceTokenCommand, string> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new RegisterDeviceTokenCommand(
                request.Token,
                request.Platform
            );

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                _ => Results.Ok(new {  request.Token }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Notifications)
        .RequireAuthorization()
        .WithDescription("    Android = 1,\r\n    IOS = 2,\r\n    Web = 3")
        ;

    }

    internal sealed class Request
    {
        public string Token { get; init; } = string.Empty;
        public DevicePlatform Platform { get; init; }
    }
}
