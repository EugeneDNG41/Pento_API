using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.SignOut;

internal sealed class SignOutCommandHandler(IJwtService jwtService) : ICommandHandler<SignOutCommand>
{
    public async Task<Result> Handle(SignOutCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.AccessToken))
        {
            return Result.Failure(IdentityProviderErrors.InvalidToken);
        }
        Result revokeAccessTokenResult = await jwtService.RevokeAccessTokenAsync(command.AccessToken, cancellationToken);
        if (revokeAccessTokenResult.IsFailure)
        {
            return Result.Failure(revokeAccessTokenResult.Error);
        }
        return Result.Success();
    }
}
