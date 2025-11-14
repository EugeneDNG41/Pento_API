using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItemLogs.GetSummary;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItemLogs;

internal sealed class GetFoodItemLogSummary : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-item-logs/summary", async (
            DateTime? fromDate,
            DateTime? toDate,
            Guid? weightUnitId,
            Guid? volumeUnitId,
            IQueryHandler<GetFoodItemLogSummaryQuery, FoodItemLogSummary> handler,
            CancellationToken cancellationToken) =>
        {
            Result<FoodItemLogSummary> result = await handler.Handle(new GetFoodItemLogSummaryQuery(
                fromDate?.ToUniversalTime(),
                toDate?.ToUniversalTime(),
                weightUnitId,
                volumeUnitId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItemLogs)
        .RequireAuthorization()
        .WithDescription("Get summary of food item logs");
    }
}
