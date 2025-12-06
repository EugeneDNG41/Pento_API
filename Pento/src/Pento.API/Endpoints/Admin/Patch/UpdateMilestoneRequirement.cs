using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Milestones.UpdateRequirement;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Patch;

internal sealed class UpdateMilestoneRequirement : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("admin/milestones/requirements/{requirementId:guid}", async (
            Guid requirementId,
            Request request,
            ICommandHandler<UpdateMilestoneRequirementCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateMilestoneRequirementCommand(
                requirementId,
                request.ActivityCode,
                request.Quota,
                request.WithinDays), cancellationToken);
            return result
            .Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageMilestones);
    }
    internal sealed class Request
    {
        public string? ActivityCode { get; init; }
        public int? Quota { get; init; }
        public int? WithinDays { get; init; }
    }
}
