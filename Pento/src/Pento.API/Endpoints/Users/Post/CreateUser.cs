using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Create;
using Pento.Application.Users.Get;
using Pento.Application.Users.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Post;

internal sealed class CreateUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("admin/users", async (
            HttpContext context,
            Request request, 
            ICommandHandler<CreateUserCommand, BasicUserResponse> handler, 
            CancellationToken cancellationToken) =>
        {
            Result<BasicUserResponse> result = await handler.Handle(new CreateUserCommand(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName), cancellationToken);
            
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(Permissions.ManageUsers)
        .WithDescription("Creates a new user.")
        .WithTags(Tags.Admin);
    }
    internal sealed class Request
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}
