using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryLists;

namespace Pento.Application.GroceryLists.Create;

internal sealed class CreateGroceryListCommandHandler(
    IGenericRepository<GroceryList> groceryListRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateGroceryListCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGroceryListCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(GroceryListErrors.ForbiddenAccess);
        }

        bool exists = await groceryListRepository.AnyAsync(
             g => g.HouseholdId == householdId &&
             g.Name == command.Name,
        cancellationToken);

        if (exists)
        {
            return Result.Failure<Guid>(GroceryListErrors.DuplicateName);
        }

        DateTime utcNow = dateTimeProvider.UtcNow;

        var groceryList = GroceryList.Create(
            householdId: householdId.Value,
            name: command.Name,
            createdBy: userContext.UserId,
            createdOnUtc: utcNow, userContext.UserId
        );

        groceryListRepository.Add(groceryList);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(groceryList.Id);
    }
}
