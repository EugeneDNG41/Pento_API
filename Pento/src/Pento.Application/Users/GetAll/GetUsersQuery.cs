using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;

namespace Pento.Application.Users.GetAll;

public sealed record GetUsersQuery(string? SearchText, bool? IsDeleted, GetUsersSortBy SortBy, SortOrder SortOrder, int PageNumber, int PageSize) : IQuery<PagedList<UserPreview>>;
public enum GetUsersSortBy
{
    Id,
    HouseholdName,
    Email,
    FirstName,
    LastName,
    CreatedAt
}
