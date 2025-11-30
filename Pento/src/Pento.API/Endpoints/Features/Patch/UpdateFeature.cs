using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Features.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.API.Endpoints.Features.Patch;

internal sealed class UpdateFeature : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("admin/features/{featureCode}", async (
            string featureCode,
            Request request,
            ICommandHandler<UpdateFeatureCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateFeatureCommand(
                featureCode,
                request.Name,
                request.Description,
                request.DefaultQuota,
                request.DefaultResetPeriod), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin);
    }
    internal sealed class Request
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public int? DefaultQuota { get; init; }
        public TimeUnit? DefaultResetPeriod { get; init; }
    }
}
