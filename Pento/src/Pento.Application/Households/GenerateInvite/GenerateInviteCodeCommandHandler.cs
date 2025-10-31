
using System.Security.Cryptography;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Households.GenerateInvite;

internal sealed class GenerateInviteCodeCommandHandler(IGenericRepository<Household> repository, IUnitOfWork unitOfWork) : ICommandHandler<GenerateInviteCodeCommand, string>
{
    public async Task<Result<string>> Handle(GenerateInviteCodeCommand command, CancellationToken cancellationToken)
    {
        if (command.HouseholdId is null)
        {
            return Result.Failure<string>(UserErrors.NotInAnyHouseHold);
        }
        Household? household = await repository.GetByIdAsync(command.HouseholdId.Value, cancellationToken);
        if (household is null)
        {
            return Result.Failure<string>(HouseholdErrors.NotFound);
        }
        string inviteCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8))
                            .Replace("+", "-")
                            .Replace("/", "_")
                            .TrimEnd('=');

        household.SetInviteCode(inviteCode, command.CodeExpiration is null ? command.CodeExpiration : command.CodeExpiration.Value.ToUniversalTime());
        repository.Update(household);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return inviteCode;
    }
}
