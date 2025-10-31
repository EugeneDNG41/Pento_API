using System.Collections.Generic;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.Households.Create;

internal sealed class CreateHouseholdCommandHandler(
    IGenericRepository<Household> householdRepository, 
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateHouseholdCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateHouseholdCommand command, CancellationToken cancellationToken)
    {
        User? user = (await userRepository.FindIncludeAsync(u => u.Id == command.UserId, u => u.Roles, cancellationToken)).SingleOrDefault();
        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }
        if (user.HouseholdId is not null)
        {
            user.LeaveHousehold(user.HouseholdId.Value);
        }
        var household = Household.Create(command.Name);       
        householdRepository.Add(household);
        user.SetHouseholdId(household.Id);
        user.SetRoles([Role.HouseholdHead]);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return household.Id;
    }
}
