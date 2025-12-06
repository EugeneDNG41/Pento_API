
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.External.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.Users.SignIn;

internal sealed class WebSignInUserCommandHandler(IGenericRepository<User> userRepository, IJwtService jwtService) : ICommandHandler<WebSignInCommand, AuthToken>
{
    public async Task<Result<AuthToken>> Handle(
        WebSignInCommand request,
        CancellationToken cancellationToken)
    {
        User? user = (await userRepository.FindIncludeAsync(
            u => u.Email == request.Email,
            u => u.Roles,
            cancellationToken)).SingleOrDefault();
        if (user == null || !user.Roles.Any(r => r.Type == RoleType.Administrative))
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
