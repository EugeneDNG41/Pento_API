using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Households.Delete;

public sealed record DeleteHouseholdCommand(Guid Id) : ICommand;
internal sealed class DeleteHouseholdCommandHandler(
    IGenericRepository<Household> householdRepository,
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteHouseholdCommand>
{
    public async Task<Result> Handle(DeleteHouseholdCommand command, CancellationToken cancellationToken)
    {
        Household? household = await householdRepository.GetByIdAsync(command.Id, cancellationToken);
        if (household == null)
        {
            return Result.Failure(HouseholdErrors.NotFound);
        }
        bool usersInHousehold = await userRepository.AnyAsync(
            u => u.HouseholdId == household.Id, cancellationToken);
        if (usersInHousehold)
        {
            return Result.Failure(HouseholdErrors.HasActiveUsers);
        }
        household.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
