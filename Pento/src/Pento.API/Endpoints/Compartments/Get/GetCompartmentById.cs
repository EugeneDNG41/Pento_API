using Microsoft.AspNetCore.Mvc;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Roles;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pento.API.Endpoints.Compartments.Get;

internal sealed class GetCompartmentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("compartments/{compartmentId:guid}", async (           
            Guid compartmentId,
            string? searchText,    
            IQueryHandler<GetCompartmentByIdQuery, CompartmentWithFoodItemPreviewResponse> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<CompartmentWithFoodItemPreviewResponse> result = await handler.Handle(
                new GetCompartmentByIdQuery(
                    compartmentId,
                    searchText,
                    pageNumber, 
                    pageSize), cancellationToken);
            return result
            .Match(compartment => Results.Ok(compartment), CustomResults.Problem);
        })
        .WithTags(Tags.Compartments)
        .RequireAuthorization()
        .WithName(RouteNames.GetCompartmentById);
    }
}
