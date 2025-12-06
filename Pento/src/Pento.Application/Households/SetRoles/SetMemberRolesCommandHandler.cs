using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.Households.SetRoles;

internal sealed class SetMemberRolesCommandHandler(
    IUserContext userContext,
    IGenericRepository<User> userRepository,
    IGenericRepository<Role> roleRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SetMemberRolesCommand>
{
    public async Task<Result> Handle(SetMemberRolesCommand command, CancellationToken cancellationToken)
    {
        Guid? currentHouseholdId = userContext.HouseholdId;
        if (currentHouseholdId is null)
        {
            return Result.Failure(HouseholdErrors.NotInAnyHouseHold);
        }
        if (command.MemberId == userContext.UserId)
        {
            return Result.Failure(HouseholdErrors.CannotAssignRolesSelf);
        }
        User? user = (await userRepository.FindIncludeAsync(u => u.Id == command.MemberId, u => u.Roles, cancellationToken)).SingleOrDefault();
        if (user is null || user.HouseholdId != currentHouseholdId.Value)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        List<Role> roles = [];
        foreach (string roleName in command.Roles)
        {
            Role? role = (await roleRepository.FindAsync(r => r.Name == roleName && r.Type == RoleType.Household, cancellationToken))
                .SingleOrDefault();
            if (role is null)
            {
                return Result.Failure(RoleErrors.NotFound);
            }
            roles.Add(role);
        }
        if (roles.Count == 0)
        {
            return Result.Failure(RoleErrors.MustHaveOne);
        }
        user.SetRoles(roles);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
