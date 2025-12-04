using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserMilestones.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetMilestoneById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/milestones/{milestoneId:guid}", async (
            Guid milestoneId,
            IQueryHandler<GetMilestoneByIdQuery, UserMilestoneDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMilestoneByIdQuery(milestoneId);
            Result<UserMilestoneDetailResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
}
