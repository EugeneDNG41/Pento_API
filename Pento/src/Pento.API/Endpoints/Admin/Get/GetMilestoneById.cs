using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Milestones.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetMilestoneById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/milestones/{milestoneId:guid}", async (
            Guid milestoneId,
            IQueryHandler<GetAdminMilestoneByIdQuery, AdminMilestoneDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<AdminMilestoneDetailResponse> result = await handler.Handle(new GetAdminMilestoneByIdQuery(milestoneId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Admin).WithName(RouteNames.GetMilestoneById).RequireAuthorization(Permissions.ManageMilestones);
    }
}
