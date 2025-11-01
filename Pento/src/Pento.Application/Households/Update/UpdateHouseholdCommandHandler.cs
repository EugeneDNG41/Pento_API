using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Households.Update;

internal sealed class UpdateHouseholdCommandHandler(
    IUserContext userContext,
    IGenericRepository<Household> repository, 
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateHouseholdCommand>
{
    public async Task<Result> Handle(UpdateHouseholdCommand command, CancellationToken cancellationToken)
    {
        Guid? currentHouseholdId = userContext.HouseholdId;
        if (currentHouseholdId is null)
        {
            return Result.Failure(UserErrors.NotInAnyHouseHold);
        }
        Household? household = await repository.GetByIdAsync(currentHouseholdId.Value, cancellationToken);
        if (household is null)
        {
            return Result.Failure(HouseholdErrors.NotFound);
        }
        household.Update(command.Name);
        repository.Update(household);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
