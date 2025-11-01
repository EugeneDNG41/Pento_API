using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Households.RemoveMember;

internal sealed class RemoveHouseholdMemberCommandHandler(
    IUserContext userContext,
    IGenericRepository<User> userRepository, 
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveHouseholdMemberCommand>
{
    public async Task<Result> Handle(RemoveHouseholdMemberCommand command, CancellationToken cancellationToken)
    {
        Guid? currentHouseholdId = userContext.HouseholdId;
        if (currentHouseholdId is null)
        {
            return Result.Failure(UserErrors.NotInAnyHouseHold);
        }
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        if (user.HouseholdId != currentHouseholdId)
        {
            return Result.Failure(UserErrors.UserNotInYourHousehold);
        }
        user.SetHouseholdId(null);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
