using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Households.Post;

internal sealed class CreateHousehold : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("households", async(
            Request request, 
            ICommandHandler<CreateHouseholdCommand, string> handler, 
            CancellationToken cancellationToken) =>
        {
            Result<string> result = await handler.Handle(
                new CreateHouseholdCommand(request.Name), cancellationToken);
            return result.Match(code => Results.CreatedAtRoute(RouteNames.GetCurrentHousehold, null, new {InviteCode = code}), CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization();
    }
    internal sealed class Request
    {
        public string Name { get; init; }
    }
}
