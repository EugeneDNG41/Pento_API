using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItemReservations.Get;
using Pento.Application.MealPlans.Reserve;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlans.Get;

internal sealed class GetMealPlanReservations : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meal-plan-reservations/current-household", async (
            IQueryHandler<GetMealPlanReservationsByHouseholdIdQuery, IReadOnlyList<MealPlanReservationResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetMealPlanReservationsByHouseholdIdQuery();

            Result<IReadOnlyList<MealPlanReservationResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.MealPlans)
        .RequireAuthorization();
    }
}
