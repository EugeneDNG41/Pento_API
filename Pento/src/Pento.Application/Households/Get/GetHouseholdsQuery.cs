using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Households.Get;

public sealed record GetHouseholdsQuery(
    string? SearchTerm, 
    bool? IsDeleted, 
    int? FromMember, 
    int? ToMember, 
    GetHouseholdsSortBy? SortBy,
    SortOrder? SortOrder,
    int PageNumber, 
    int PageSize) : IQuery<PagedList<HouseholdPreview>>;
public enum GetHouseholdsSortBy
{
    Name,
    Members
}
