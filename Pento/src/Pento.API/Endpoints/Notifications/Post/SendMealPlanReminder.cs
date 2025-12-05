using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Notifications.Send;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;

namespace Pento.API.Endpoints.Notifications;

internal sealed class SendMealPlanReminder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("notifications/mealplan/reminder", async (
            Request request,
            ICommandHandler<SendMealPlanReminderCommand> handler,
            CancellationToken ct
        ) =>
        {
            var command = new SendMealPlanReminderCommand(request.MealPlanId);

            Result result = await handler.Handle(command, ct);

            return result.Match(
                Results.NoContent,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Notifications)
        .RequireAuthorization();
    }

    internal sealed class Request
    {
        public Guid MealPlanId { get; init; }
    }
}
