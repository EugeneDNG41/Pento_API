using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Get;

public sealed record GetAllFoodReferencesQuery(
    FoodGroup? FoodGroup,
    string? Search,
    int Page = 1,
    int PageSize = 10
) : IQuery<PagedList<FoodReferenceResponse>>;

