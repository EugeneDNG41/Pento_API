using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Activities.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Activity.Patch;

internal sealed class UpdateActivity : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("admin/activities/{activityCode:guid}", async (
            string activityCode,
            Request request,
            ICommandHandler<UpdateActivityCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateActivityCommand(activityCode, request.Name, request.Description), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin);
    }
    internal sealed class Request
    {
        public string? Name { get; init; }
        public string? Description { get; init; }

    }
}
