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
            IUserContext userContext,
            Request request, 
            ICommandHandler<CreateHouseholdCommand, Guid> handler, 
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(
                new CreateHouseholdCommand(request.Name, userContext.UserId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Households).RequireAuthorization();
    }
    internal sealed class Request
    {
        public string Name { get; init; }
    }
}
