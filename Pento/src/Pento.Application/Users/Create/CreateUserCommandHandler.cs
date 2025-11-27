using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Get;
using Pento.Application.Users.Search;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.Create;

internal sealed class CreateUserCommandHandler(
    IIdentityProviderService identityProviderService,
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateUserCommand, BasicUserResponse>
{
    public async Task<Result<BasicUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        bool emailTaken = await userRepository.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (emailTaken)
        {
            return Result.Failure<BasicUserResponse>(IdentityProviderErrors.EmailTaken);
        }
        Result<string> result = await identityProviderService.RegisterUserAsync(
            new UserModel(request.Email, request.Password, request.FirstName, request.LastName),
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<BasicUserResponse>(result.Error);
        }

        var user = User.Create(request.Email, request.FirstName, request.LastName, result.Value, dateTimeProvider.UtcNow);

        userRepository.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new BasicUserResponse(user.Id, user.FirstName, user.LastName, user.AvatarUrl);
    }
}
