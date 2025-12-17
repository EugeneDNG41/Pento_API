using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
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
        User? member = (await userRepository.FindIncludeAsync(u => u.Id == command.UserId, u => u.Roles, cancellationToken)).SingleOrDefault();
        if (member == null || member.HouseholdId != currentHouseholdId)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        member.SetHouseholdId(null);
        member.SetRoles(new List<Role>());
        await userRepository.UpdateAsync(member, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
