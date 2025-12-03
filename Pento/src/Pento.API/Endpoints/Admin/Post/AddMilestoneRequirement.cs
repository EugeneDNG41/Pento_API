using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Milestones.AddRequirement;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Post;

internal sealed class AddMilestoneRequirement : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("admin/milestones/{milestoneId:guid}/requirements", async (
            Guid milestoneId,
            Request request,
            ICommandHandler<AddMilestoneRequirementCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(new AddMilestoneRequirementCommand(
                milestoneId,
                request.ActivityCode,
                request.Quota,
                request.WithinDays), cancellationToken);
            return result
            .Match(id => Results.CreatedAtRoute(RouteNames.GetMilestoneById, new { milestoneId }, new { milestoneId = id }), CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageMilestones);
    }
    internal sealed class Request
    {
        public string ActivityCode { get; init; }
        public int Quota { get; init; }
        public int WithinDays { get; init; }
    }
}
