using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.Delete;

internal sealed class DeleteUserCommandHandler(
    IUserContext userContext,
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        if (user.Id == userContext.UserId)
        {
            return Result.Failure(UserErrors.CannotDeleteSelf);
        }
        if (user.IsDeleted)
        {
            return Result.Failure(UserErrors.AlreadyDeleted);
        }
        await userRepository.RemoveAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

}
