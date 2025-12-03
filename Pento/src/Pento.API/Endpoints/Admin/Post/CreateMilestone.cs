using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Milestones.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Post;

internal sealed class CreateMilestone : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("admin/milestones", async (
            Request request,
            ICommandHandler<CreateMilestoneCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(new CreateMilestoneCommand(
                request.Name,
                request.Description,
                request.IsActive), cancellationToken);
            return result
            .Match(id => Results.CreatedAtRoute(RouteNames.GetMilestoneById, new { milestoneId = id }, id), CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageMilestones);
    }
    internal sealed class Request
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public bool IsActive { get; init; }
    }
}
