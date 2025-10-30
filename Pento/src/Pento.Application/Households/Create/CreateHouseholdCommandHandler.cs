using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Households.Create;

internal sealed class CreateHouseholdCommandHandler(
    IGenericRepository<Household> repository, 
    IGenericRepository<User> userRepository,
    IUserRepository repo,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateHouseholdCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateHouseholdCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }
        if (user.HouseholdId is not null)
        {
            user.LeaveHousehold(user.HouseholdId.Value);
        }
        var household = Household.Create(command.Name);       
        repository.Add(household);
        user.SetHouseholdId(household.Id);
        user.SetRoles(new[] { Role.HouseholdAdmin });
        repo.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return household.Id;
    }
}
