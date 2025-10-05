
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.SignIn;

internal sealed class SignInUserCommandHandler(IIdentityProviderService identityProviderService) : ICommandHandler<SignInUserCommand, AuthToken>
{
    public async Task<Result<AuthToken>> Handle(
        SignInUserCommand request,
        CancellationToken cancellationToken)
    {
        Result<AuthToken> result = await identityProviderService.GetAuthTokenAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<AuthToken>(UserErrors.InvalidCredentials);
        }

        return result.Value;
    }
}
