using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Wishlist.Add;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.RecipeWishLists;
using Pento.Domain.Users;

namespace Pento.Application.RecipeWishLists.Add;

internal sealed class AddRecipeToWishListCommandHandler(
    IGenericRepository<RecipeWishList> repository,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<AddRecipeToWishListCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddRecipeToWishListCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        IEnumerable<RecipeWishList> exists = await repository.FindAsync(
            x => x.HouseholdId == householdId.Value &&
                 x.RecipeId == command.RecipeId,
            cancellationToken);

        if (exists.Any())
        {
            return Result.Failure<Guid>(RecipeWishListErrors.AlreadyExists);
        }

        var wishlist = RecipeWishList.Create(householdId.Value, command.RecipeId);

        repository.Add(wishlist);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return wishlist.Id;
    }
}
