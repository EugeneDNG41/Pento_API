using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.MealPlans.Reserve;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;

namespace Pento.API.Endpoints.MealPlans.Get;

internal sealed class GetMealPlanReservations : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meal-plan-reservations", async (
            string? mealType,
            DateOnly? date,
            int? month,
            int? year,
            ReservationStatus? status,
            Guid? foodReferenceId,
            int pageNumber,
            int pageSize,
            IQueryHandler<GetMealPlanReservationsByHouseholdQuery, PagedList<MealPlanReservationResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetMealPlanReservationsByHouseholdQuery(
                MealType: mealType,
                Date: date,
                Month: month,
                Year: year,
                Status: status,
                FoodReferenceId: foodReferenceId,
                PageNumber: pageNumber <= 0 ? 1 : pageNumber,
                PageSize: pageSize <= 0 ? 10 : pageSize
            );

            Result<PagedList<MealPlanReservationResponse>> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Reservations)
        .RequireAuthorization();
    }
}

