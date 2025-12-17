using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryLists;

namespace Pento.Application.GroceryLists.Update;

internal sealed class UpdateGroceryListCommandHandler(
    IUserContext userContext,
    IGenericRepository<GroceryList> groceryListRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdateGroceryListCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateGroceryListCommand command, CancellationToken cancellationToken)
    {
        GroceryList? list = await groceryListRepository.GetByIdAsync(command.Id, cancellationToken);
        if (list is null)
        {
            return Result.Failure<Guid>(GroceryListErrors.NotFound);
        }

        if (list.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure<Guid>(GroceryListErrors.ForbiddenAccess);
        }

        bool nameExists = await groceryListRepository.AnyAsync(
            g => g.HouseholdId == list.HouseholdId &&
                 g.Id != list.Id &&
                 g.Name == command.Name,
            cancellationToken);

        if (nameExists)
        {
            return Result.Failure<Guid>(GroceryListErrors.DuplicateName);
        }

        if (!string.Equals(list.Name, command.Name, StringComparison.Ordinal))
        {
            list.UpdateName(command.Name, dateTimeProvider.UtcNow);
            await groceryListRepository.UpdateAsync(list, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(list.Id);
    }
}
