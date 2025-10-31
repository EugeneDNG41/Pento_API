using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.Households.SetRoles;

internal sealed class SetMemberRolesCommandHandler(
    IGenericRepository<User> userRepository,
    IGenericRepository<Role> roleRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SetMemberRolesCommand>
{
    public async Task<Result> Handle(SetMemberRolesCommand command, CancellationToken cancellationToken)
    {
        if (command.HouseholdId is null)
        {
            return Result.Failure(UserErrors.NotInAnyHouseHold);
        }
        User? user = (await userRepository.FindIncludeAsync(u => u.Id == command.MemberId, u => u.Roles, cancellationToken)).SingleOrDefault();
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        if (user.HouseholdId != command.HouseholdId)
        {
            return Result.Failure(UserErrors.UserNotInYourHousehold);
        }
        List<Role> roles = new();
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
