using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;

namespace Pento.Application.Households.RevokeInvite;

internal sealed class RevokeInviteCodeCommandHandler(IGenericRepository<Household> repository, IUnitOfWork unitOfWork) : ICommandHandler<RevokeInviteCodeCommand>
{
    public async Task<Result> Handle(RevokeInviteCodeCommand command, CancellationToken cancellationToken)
    {
        Household? household = await repository.GetByIdAsync(command.HouseholdId, cancellationToken);
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
