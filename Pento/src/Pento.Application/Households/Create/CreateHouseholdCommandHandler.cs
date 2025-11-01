using System.Collections.Generic;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.Households.Create;

internal sealed class CreateHouseholdCommandHandler(
    IUserContext userContext,
    IGenericRepository<Household> householdRepository, 
    IGenericRepository<User> userRepository,
    IGenericRepository<Role> roleRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateHouseholdCommand, string>
{
    public async Task<Result<string>> Handle(CreateHouseholdCommand command, CancellationToken cancellationToken)
    {
        User? currentUser = (await userRepository.FindIncludeAsync(u => u.Id == userContext.UserId, u => u.Roles, cancellationToken)).SingleOrDefault();        
        if (currentUser is null)
        {
            return Result.Failure<string>(UserErrors.NotFound);
        }
        //deal with previous household if exists
        Role? householdHeadRole = (await roleRepository.FindAsync(r => r.Name == Role.HouseholdHead.Name, cancellationToken)).Single();
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
        //new household
        var household = Household.Create(command.Name);
        householdRepository.Add(household);
        currentUser.SetHouseholdId(household.Id);       
        currentUser.SetRoles([householdHeadRole]);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return household.InviteCode;
    }
}
