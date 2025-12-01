using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;

namespace Pento.Application.UserActivities.GetCurrentActivities;

public sealed record GetCurrentActivitiesQuery(string? SearchTerm, DateTime? FromDate, DateTime? ToDate, int PageNumber, int PageSize) : IQuery<PagedList<CurrentUserActivityResponse>>;
