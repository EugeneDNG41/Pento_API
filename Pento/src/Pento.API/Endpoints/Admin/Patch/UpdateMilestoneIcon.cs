using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Milestones.UpdateIcon;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Patch;

internal sealed class UpdateMilestoneIcon : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("admin/milestones/{milestoneId:guid}/icon", async (
            Guid milestoneId,
            IFormFile icon,
            ICommandHandler<UpdateMilestoneIconCommand, Uri> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Uri> result = await handler.Handle(new UpdateMilestoneIconCommand(
                milestoneId,
                icon), cancellationToken);
            return result
            .Match((uri) => Results.Ok(new {IconUrl = uri}), CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageMilestones).DisableAntiforgery();
    }
}
