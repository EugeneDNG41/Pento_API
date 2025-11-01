
using System.Security.Cryptography;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Households.GenerateInvite;

internal sealed class GenerateInviteCodeCommandHandler(
    IUserContext userContext,
    IGenericRepository<Household> repository, 
    IUnitOfWork unitOfWork) : ICommandHandler<GenerateInviteCodeCommand, string>
{
    public async Task<Result<string>> Handle(GenerateInviteCodeCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<string>(UserErrors.NotInAnyHouseHold);
        }
        Household? household = await repository.GetByIdAsync(householdId.Value, cancellationToken);
        if (household is null)
        {
            return Result.Failure<string>(HouseholdErrors.NotFound);
        }
        household.GenerateInviteCode();
        household.SetInviteCodeExpiration(
            command.CodeExpiration is null ? 
            command.CodeExpiration : 
            command.CodeExpiration.Value.ToUniversalTime());
        repository.Update(household);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return household.InviteCode;
    }
}
