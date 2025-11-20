using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Get;

public sealed record GetMealPlansByHouseholdIdQuery(
    int PageNumber = 1,
    int PageSize = 10,
    DateOnly? Date = null,        
    int? Month = null,            
    int? Year = null,            
    MealType? MealType = null,    
    bool SortAsc = false          
) : IQuery<PagedList<MealPlanResponse>>;
