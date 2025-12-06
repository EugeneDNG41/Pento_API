using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.FoodItemReservations;

namespace Pento.Application.MealPlans.Reserve;
public sealed record GetMealPlanReservationsByHouseholdQuery(
    string? MealType,
    DateOnly? Date,
    int? Month,
    int? Year,
    ReservationStatus? Status,
    Guid? FoodReferenceId,
    int PageNumber,
    int PageSize
) : IQuery<PagedList<MealPlanReservationResponse>>;

