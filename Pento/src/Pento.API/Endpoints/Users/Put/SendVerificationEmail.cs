
using Pento.API.Extensions;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Identity;

using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Put;

internal sealed class SendVerificationEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/send-verification-email", async (
            IUserContext context,
            IIdentityProviderService service,
            CancellationToken cancellationToken) =>
        {
            Result result = await service.SendVerificationEmailAsync(context.IdentityId, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags("Users");
    }
}
