using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Milestones.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Patch;

internal sealed class UpdateMilestone : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("admin/milestones/{milestoneId:guid}", async (
            Guid milestoneId,
            Request request,
            ICommandHandler<UpdateMilestoneCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateMilestoneCommand(
                milestoneId,
                request.Name,
                request.Description,
                request.IsActive), cancellationToken);
            return result
            .Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageMilestones);
    }
    internal sealed class Request
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public bool? IsActive { get; init; }
    }
}
