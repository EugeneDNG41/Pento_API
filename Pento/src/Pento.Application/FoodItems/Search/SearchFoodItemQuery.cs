using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Marten;
using Marten.Pagination;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Projections;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.FoodItems.Search;

public sealed record SearchFoodItemQuery(
    string? SearchText,
    List<FoodGroup> FoodGroups,
    decimal? FromQuantity,
    decimal? ToQuantity,
    DateTime? ExpirationDateAfter,
    DateTime? ExpirationDateBefore,
    int PageNumber,
    int PageSize) : IQuery<IPagedList<FoodItemPreview>>;

internal sealed class SearchFoodItemQueryValidator : AbstractValidator<SearchFoodItemQuery>
{
    public SearchFoodItemQueryValidator()
    {
        RuleFor(x => x.FoodGroups).Must(fg => fg.Distinct().Count() == fg.Count)
            .WithMessage("Food group filters must be distinct.");
    }
}

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
        IReadOnlyList<Guid> ids = await session.Query<FoodItem>().Where(f => f.HouseholdId == householdId && f.Quantity > 0).Select(f => f.Id).ToListAsync(cancellationToken);
        IPagedList<FoodItemPreview> previews =
               await session.Query<FoodItemPreview>()
                   .Where(p => ids.Contains(p.Id))
                   .Where(p => string.IsNullOrEmpty(query.SearchText) || p.WebStyleSearch(query.SearchText))
                   .Where(p => query.FoodGroups == null || query.FoodGroups.Contains(p.FoodGroup))
                   .Where(p => query.FromQuantity == null || p.Quantity >= query.FromQuantity)
                   .Where(p => query.ToQuantity == null || p.Quantity <= query.ToQuantity)
                   .Where(p => query.ExpirationDateAfter == null || p.ExpirationDateUtc >= query.ExpirationDateAfter.Value.ToUniversalTime())
                   .Where(p => query.ExpirationDateBefore == null || p.ExpirationDateUtc <= query.ExpirationDateBefore.Value.ToUniversalTime())
                   .ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        return Result.Success(previews);
    }
}
