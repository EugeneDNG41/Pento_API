using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Households.RevokeInvite;

internal sealed class RevokeInviteCodeCommandHandler(
    IUserContext userContext,
    IGenericRepository<Household> repository, 
    IUnitOfWork unitOfWork) : ICommandHandler<RevokeInviteCodeCommand>
{
    public async Task<Result> Handle(RevokeInviteCodeCommand command, CancellationToken cancellationToken)
    {
        Guid? currentHouseholdId = userContext.HouseholdId;
        if (currentHouseholdId is null)
        {
            return Result.Failure(HouseholdErrors.NotInAnyHouseHold);
        }
        Household? household = await repository.GetByIdAsync(currentHouseholdId.Value, cancellationToken);
        if (household is null)
        {
            return Result.Failure(HouseholdErrors.NotFound);
        }
        household.RevokeInviteCode();
        repository.Update(household);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
