using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.Households.Join;

internal sealed class JoinHouseholdCommandHandler(
    IUserContext userContext,
    IGenericRepository<Household> householdRepository, 
    IGenericRepository<User> userRepository,
    IGenericRepository<Role> roleRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<JoinHouseholdCommand>
{
    public async Task<Result> Handle(JoinHouseholdCommand command, CancellationToken cancellationToken)
    {
        User? currentUser = await userRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (currentUser is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        //check valid invite code
        Household? household = (await householdRepository.FindAsync(h => h.InviteCode == command.InviteCode, cancellationToken)).SingleOrDefault();
        if (household is null)
        {
            return Result.Failure(HouseholdErrors.InviteCodeNotFound);
        }
        if (household.InviteCodeExpirationUtc is not null && household.InviteCodeExpirationUtc < DateTime.UtcNow)
        {
            return Result.Failure(HouseholdErrors.InviteCodeExpired);
        }

        //assign roles
        Role? householdHeadRole = (await roleRepository.FindAsync(r => r.Name == Role.HouseholdHead.Name, cancellationToken)).SingleOrDefault();
        Role? errandRunnerRole = (await roleRepository.FindAsync(r => r.Name == Role.ErrandRunner.Name, cancellationToken)).SingleOrDefault();
        if (householdHeadRole is null || errandRunnerRole is null)
        {
            return Result.Failure(RoleErrors.NotFoundToAssign);
        }
        if (currentUser.HouseholdId is not null)
        {
            IEnumerable<User> otherMembers = await userRepository.FindIncludeAsync(
                u => u.HouseholdId == currentUser.HouseholdId && u.Id != currentUser.Id,
                u => u.Roles,
                cancellationToken);

            bool hasHead = otherMembers.Any(m =>
                m.Roles.Any(r => r.Name == householdHeadRole.Name));
            if (!hasHead && otherMembers.Any())
            {
                User newHead = otherMembers.First();
                newHead.SetRoles([householdHeadRole]);
            }
        }
        
        currentUser.SetHouseholdId(household.Id);
        userRepository.Update(currentUser);
        currentUser.SetRoles([Role.ErrandRunner]);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
