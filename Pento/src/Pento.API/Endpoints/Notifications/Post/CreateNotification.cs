using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Notifications.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;

namespace Pento.API.Endpoints.Notifications;

internal sealed class CreateNotification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("notifications/send", async (
            Request request,
            ICommandHandler<CreateNotificationCommand, Guid> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new CreateNotificationCommand(
                request.Title,
                request.Body,
                request.Type,
                request.DataJson
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id, Message = "Notification sent" }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Notifications)
        .RequireAuthorization();

    }

    internal sealed class Request
    {
        public string Title { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public NotificationType Type { get; init; } = NotificationType.General;
        public string? DataJson { get; init; }
    }
}
