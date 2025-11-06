using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.UploadAvatar;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Put;

internal sealed class UploadAvatar : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/avatar",
            async (IFormFile file, ICommandHandler<UploadAvatarCommand, Uri> handler, CancellationToken ct) =>
            {
                Result<Uri> result = await handler.Handle(new UploadAvatarCommand(file), ct);
                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Users);
    }
}

