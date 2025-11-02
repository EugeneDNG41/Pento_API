using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;
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
            return Result.Failure(HouseholdErrors.NotInAnyHouseHold);
        }
        User? user = (await userRepository.FindIncludeAsync(u => u.Id == userContext.UserId, u => u.Roles, cancellationToken)).SingleOrDefault();
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        if (user.HouseholdId != currentHouseholdId)
        {
            return Result.Failure(HouseholdErrors.UserNotInYourHousehold);
        }
        user.SetHouseholdId(null);
        user.SetRoles(new List<Role>());
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
