using FluentValidation;
using Marten;
using Marten.Pagination;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Projections;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;

namespace Pento.Application.FoodItems.Search;

public sealed record SearchFoodItemQuery(
    string? SearchText,
    List<string> FoodGroups,
    decimal? FromQuantity,
    decimal? ToQuantity,
    DateTime? ExpirationDateAfter,
    DateTime? ExpirationDateBefore,
    int PageNumber,
    int PageSize) : IQuery<IPagedList<FoodItemPreview>>;

internal sealed class SearchFoodItemQueryHandler(
    IUserContext userContext, 
    IQuerySession session) : IQueryHandler<SearchFoodItemQuery, IPagedList<FoodItemPreview>>
{
    public async Task<Result<IPagedList<FoodItemPreview>>> Handle(SearchFoodItemQuery query, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<IPagedList<FoodItemPreview>>(HouseholdErrors.NotInAnyHouseHold);
        }
        IReadOnlyList<Guid> ids = await session.Query<FoodItem>()
            .Where(f => f.HouseholdId == householdId && f.Quantity > 0)
            .Select(f => f.Id).ToListAsync(cancellationToken);
        IQueryable<FoodItemPreview> previewsQuery =
                session.Query<FoodItemPreview>()
                   .Where(p => ids.Contains(p.Id))
                   .Where(p => !query.FoodGroups.Any()|| query.FoodGroups.Contains(p.FoodGroup))
                   .Where(p => query.FromQuantity == null || p.Quantity >= query.FromQuantity)
                   .Where(p => query.ToQuantity == null || p.Quantity <= query.ToQuantity)
                   .Where(p => query.ExpirationDateAfter == null || p.ExpirationDateUtc >= query.ExpirationDateAfter.Value.ToUniversalTime())
                   .Where(p => query.ExpirationDateBefore == null || p.ExpirationDateUtc <= query.ExpirationDateBefore.Value.ToUniversalTime());
        if (!string.IsNullOrEmpty(query.SearchText))
        {
            string trimmed = query.SearchText.Trim();
            previewsQuery = previewsQuery.Where(p => p.Name.Contains(trimmed) || p.FoodGroup.Contains(trimmed));
        }
        IPagedList<FoodItemPreview> previews = await previewsQuery.ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        return Result.Success(previews);
    }
}
