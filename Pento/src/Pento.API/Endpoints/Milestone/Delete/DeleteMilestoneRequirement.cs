using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Milestones.DeleteRequirement;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Milestone.Delete;

internal sealed class DeleteMilestoneRequirement : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("admin/milestones/requirements/{requirementId:guid}", async (
            Guid requirementId,
            ICommandHandler<DeleteMilestoneRequirementCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new DeleteMilestoneRequirementCommand(requirementId), cancellationToken);
            return result
            .Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageMilestones);
    }
}
