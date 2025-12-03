using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Milestones.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Delete;

internal sealed class DeleteMilestone : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("admin/milestones/{milestoneId:guid}", async (
            Guid milestoneId,
            ICommandHandler<DeleteMilestoneCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new DeleteMilestoneCommand(milestoneId), cancellationToken);
            return result
            .Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageMilestones);
    }
}
