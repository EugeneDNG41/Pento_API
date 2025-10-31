using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Households.RevokeInvite;

internal sealed class RevokeInviteCodeCommandHandler(IGenericRepository<Household> repository, IUnitOfWork unitOfWork) : ICommandHandler<RevokeInviteCodeCommand>
{
    public async Task<Result> Handle(RevokeInviteCodeCommand command, CancellationToken cancellationToken)
    {
        if (command.HouseholdId is null)
        {
            return Result.Failure(UserErrors.NotInAnyHouseHold);
        }
        Household? household = await repository.GetByIdAsync(command.HouseholdId.Value, cancellationToken);
        if (household is null)
        {
            return Result.Failure(HouseholdErrors.NotFound);
        }
        household.SetInviteCode(null, null);
        repository.Update(household);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
