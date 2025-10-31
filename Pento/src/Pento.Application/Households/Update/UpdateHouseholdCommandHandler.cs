using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Households.Update;

internal sealed class UpdateHouseholdCommandHandler(IGenericRepository<Household> repository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateHouseholdCommand>
{
    // Implementation of the command handler goes here
    public async Task<Result> Handle(UpdateHouseholdCommand command, CancellationToken cancellationToken)
    {
        if (command.HouseholdId is null)
        {
            return Result.Failure(UserErrors.NotInAnyHouseHold);
        }
        Household? household = await repository.GetByIdAsync(command.HouseholdId.Value, cancellationToken);
        if (household is null)
        {
            throw new Exception("Household not found.");
        }
        household.Update(command.Name);
        repository.Update(household);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
