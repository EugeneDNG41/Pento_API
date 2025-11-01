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
    IUnitOfWork unitOfWork) : ICommandHandler<CreateHouseholdCommand, string>
{
    public async Task<Result<string>> Handle(CreateHouseholdCommand command, CancellationToken cancellationToken)
    {
        User? user = (await userRepository.FindIncludeAsync(u => u.Id == userContext.UserId, u => u.Roles, cancellationToken)).SingleOrDefault();
        if (user is null)
        {
            return Result.Failure<string>(UserErrors.NotFound);
        }
        if (user.HouseholdId is not null)
        {
            user.LeaveHousehold(user.HouseholdId.Value);
        }
        var household = Household.Create(command.Name);
        household.GenerateInviteCode();
        householdRepository.Add(household);
        user.SetHouseholdId(household.Id);
        user.SetRoles([Role.HouseholdHead]);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return household.InviteCode;
    }
}
