
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.SignIn;

internal sealed class MobileSignInCommandHandler(IGenericRepository<User> userRepository, IJwtService jwtService) : ICommandHandler<MobileSignInCommand, AuthToken>
{
    public async Task<Result<AuthToken>> Handle(
        MobileSignInCommand request,
        CancellationToken cancellationToken)
    {
        User? user = (await userRepository.FindAsync(
            u => u.Email == request.Email,
            cancellationToken)).SingleOrDefault();
        if (user == null)
        {
            return Result.Failure<AuthToken>(UserErrors.InvalidCredentials);
        }
        Result<AuthToken> result = await jwtService.GetAuthTokenAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<AuthToken>(result.Error);
        }

        return result.Value;
    }
}
