using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Households.Leave;

internal sealed class LeaveHouseholdCommandHandler(
    IUserContext userContext,
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<LeaveHouseholdCommand>
{
    public async Task<Result> Handle(LeaveHouseholdCommand command, CancellationToken cancellationToken)
    {
        Guid currentUserId = userContext.UserId;
        Guid? currentHouseholdId = userContext.HouseholdId;
        if (currentHouseholdId is null)
        {
            return Result.Success();
        }
        User? user = await userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }
        user.SetHouseholdId(null);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
