using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Identity;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.ResetPassword;

internal sealed class ResetUserPasswordCommandHandler(
    IGenericRepository<User> userRepository, 
    IIdentityProviderService identityService) : ICommandHandler<ResetUserPasswordCommand>
{
    public async Task<Result> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        User? user = (await userRepository.FindAsync(u => u.Email == request.Email, cancellationToken)).FirstOrDefault();
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        await identityService.SendResetPasswordEmailAsync(user.IdentityId, cancellationToken);
        return Result.Success();
    }
}
