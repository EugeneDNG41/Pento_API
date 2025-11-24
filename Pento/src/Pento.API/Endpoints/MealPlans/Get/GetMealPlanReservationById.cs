using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItemReservations.Get;
using Pento.Application.MealPlans.Reserve;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlanReservations.Get;

internal sealed class GetMealPlanReservationById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meal-plan-reservations/{id:guid}", async (
            Guid id,
            IQueryHandler<GetMealPlanReservationByIdQuery, MealPlanReservationDetailResponse> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetMealPlanReservationByIdQuery(id);

            Result<MealPlanReservationDetailResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Reservations)
        .RequireAuthorization();
    }
}
