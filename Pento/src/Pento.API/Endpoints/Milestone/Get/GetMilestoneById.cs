using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Milestones.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Milestone.Get;

internal sealed class GetMilestoneById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("milestones/milestones/{requirementId:guid}", async (
            Guid milestoneId,
            IQueryHandler<GetMilestoneByIdQuery, MilestoneDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<MilestoneDetailResponse> result = await handler.Handle(new GetMilestoneByIdQuery(milestoneId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Milestones).WithName(RouteNames.GetMilestoneById);
    }
}
