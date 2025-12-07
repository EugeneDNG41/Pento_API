using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Reserve;

public sealed record GetMealPlanReservationByIdQuery(Guid Id)
    : IQuery<MealPlanReservationDetailResponse>;
