using System.Threading;
using Microsoft.AspNetCore.Routing;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItemLogs.GetById;
using Pento.Domain.Abstractions;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Pento.API.Endpoints.FoodItemLogs;

internal sealed class GetFoodItemLogById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-item-logs/{foodItemLogId:guid}", async (
            Guid foodItemLogId,
            IQueryHandler<GetFoodItemLogByIdQuery, FoodItemLogDetail> handler,
            CancellationToken cancellationToken) =>
        {
            Result<FoodItemLogDetail> result = await handler.Handle(new GetFoodItemLogByIdQuery(foodItemLogId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItemLogs)
        .RequireAuthorization()
        .WithDescription("Get a specific food item log by its Id");
    }
}
