using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.RecipeWishLists;

namespace Pento.Application.Recipes.Wishlist.Remove;

internal sealed class RemoveRecipeFromWishListCommandHandler(
    IGenericRepository<RecipeWishList> repository,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<RemoveRecipeFromWishListCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        RemoveRecipeFromWishListCommand command,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        IEnumerable<RecipeWishList> items = await repository.FindAsync(
            x => x.HouseholdId == householdId.Value &&
                 x.RecipeId == command.RecipeId &&
                 !x.IsDeleted,
            cancellationToken);

        RecipeWishList? wishlist = items.FirstOrDefault();

        if (wishlist is null)
        {
            return Result.Failure<Guid>(RecipeWishListErrors.NotFound);
        }
        await repository.RemoveAsync(wishlist, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return wishlist.Id;
    }
}
