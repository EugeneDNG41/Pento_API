using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.Households.Join;

internal sealed class JoinHouseholdCommandHandler(
    IGenericRepository<Household> householdRepository, 
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<JoinHouseholdCommand>
{
    public async Task<Result> Handle(JoinHouseholdCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        User? user = await userRepository.GetByIdAsync(command.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        if (user.HouseholdId is not null)
        {
            return Result.Failure(UserErrors.UserAlreadyInHousehold);
        }
        Household? household = (await householdRepository.FindAsync(h => h.InviteCode == command.InviteCode, cancellationToken)).SingleOrDefault();
        if (household is null)
        {
            return Result.Failure(HouseholdErrors.InviteCodeNotFound);
        }
        if (household.InviteCodeExpirationUtc is not null && household.InviteCodeExpirationUtc < DateTime.UtcNow)
        {
            return Result.Failure(HouseholdErrors.InviteCodeExpired);
        }
        user.SetHouseholdId(household.Id);

        userRepository.Update(user);
        user.SetRoles([Role.ErrandRunner]);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
