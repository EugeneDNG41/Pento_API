using Pento.API.Extensions;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Create;
using Pento.Application.Users.Get;
using Pento.Application.Users.Register;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Post;

internal sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (
            HttpContext context,
            Request request, 
            ICommandHandler<RegisterUserCommand, AuthToken> handler, 
            CancellationToken cancellationToken) =>
        {
            Result<AuthToken> result = await handler.Handle(new RegisterUserCommand(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName), cancellationToken);
            
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Users);
    }

    internal sealed class Request
    {
        public string Email { get; init; }

        public string Password { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }
    }
}
internal sealed class CreateUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users", async (
            HttpContext context,
            Request request, 
            ICommandHandler<CreateUserCommand, UserResponse> handler, 
            CancellationToken cancellationToken) =>
        {
            Result<UserResponse> result = await handler.Handle(new CreateUserCommand(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName), cancellationToken);
            
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(Permissions.ManageUsers)
        .WithTags(Tags.Users);
    }
    internal sealed class Request
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}
