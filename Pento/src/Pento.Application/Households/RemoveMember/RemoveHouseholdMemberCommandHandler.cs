using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Households.RemoveMember;

internal sealed class RemoveHouseholdMemberCommandHandler(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork) : ICommandHandler<RemoveHouseholdMemberCommand>
{
    public async Task<Result> Handle(RemoveHouseholdMemberCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null || user.HouseholdId != command.HouseholdId)
        {
            return Result.Failure(UserErrors.UserNotInHousehold);
        }
        user.SetHouseholdId(null);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
