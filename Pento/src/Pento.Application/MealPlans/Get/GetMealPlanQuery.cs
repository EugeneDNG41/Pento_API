using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Get;

public sealed record GetMealPlanQuery(Guid Id) : IQuery<MealPlanDetailResponse>;

