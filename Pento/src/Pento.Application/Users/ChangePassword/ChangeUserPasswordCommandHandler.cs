using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.Identity;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.ChangePassword;

internal sealed class ChangeUserPasswordCommandHandler(
    IUserContext userContext,
    IGenericRepository<User> userRepository,
    IJwtService jwtService,
    IIdentityProviderService identityService) : ICommandHandler<ChangeUserPasswordCommand>
{
    public async Task<Result> Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        Result<AuthToken> auth = await jwtService.GetAuthTokenAsync(user.Email, command.CurrentPassword, cancellationToken);
        if (auth.IsFailure)
        {
            return Result.Failure(auth.Error);
        }
        await identityService.ChangePasswordAsync(user.IdentityId, command.NewPassword, cancellationToken);
        return Result.Success();
    }
}
