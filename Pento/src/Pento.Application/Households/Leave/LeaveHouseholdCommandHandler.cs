using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Households.Leave;

internal sealed class LeaveHouseholdCommandHandler(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork) : ICommandHandler<LeaveHouseholdCommand>
{
    public async Task<Result> Handle(LeaveHouseholdCommand command, CancellationToken cancellationToken)
    {
        if (command.HouseholdId is null)
        {
            return Result.Failure(UserErrors.NotInAnyHouseHold);
        }
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        if (user.HouseholdId != command.HouseholdId)
        {
            return Result.Failure(UserErrors.UserNotInYourHousehold);
        }
        user.LeaveHousehold(command.HouseholdId.Value);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
