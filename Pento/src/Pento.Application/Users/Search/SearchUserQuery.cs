using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;

namespace Pento.Application.Users.Search;

public sealed record SearchUserQuery(string? SearchText, bool? IsDeleted, int PageNumber, int PageSize) : IQuery<PagedList<UserPreview>>;
