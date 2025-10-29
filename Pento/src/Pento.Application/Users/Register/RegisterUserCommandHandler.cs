using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.Register;

internal sealed class RegisterUserCommandHandler(
    IIdentityProviderService identityProviderService,
    IJwtService jwtService,
    IDateTimeProvider dateTimeProvider,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterUserCommand, AuthToken>
{
    public async Task<Result<AuthToken>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Result<string> result = await identityProviderService.RegisterUserAsync(
            new UserModel(request.Email, request.Password, request.FirstName, request.LastName),
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<AuthToken>(result.Error);
        }

        var user = User.Create(request.Email, request.FirstName, request.LastName, result.Value, dateTimeProvider.UtcNow);

        userRepository.Insert(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        Result<AuthToken> tokenResult = await jwtService.GetAuthTokenAsync(user.Email, request.Password, cancellationToken);
        if (tokenResult.IsFailure)
        {
            return Result.Failure<AuthToken>(tokenResult.Error);
        }
        Result emailSentResult = await identityProviderService.SendVerificationEmailAsync(result.Value, cancellationToken);
        if (emailSentResult.IsFailure)
        {
            return Result.Failure<AuthToken>(UserErrors.VerificationEmailFailed);
        }
        return tokenResult;
    }
}
