using MediatR;
using Pento.API.Extensions;
using Pento.Application.MealPlans.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlans;

internal sealed class CreateMealPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("meal-plans", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateMealPlanCommand(
                request.HouseholdId,
                request.Name,
                request.CreatedBy,
                request.StartDate,
                request.EndDate
            );

            Result<Guid> result = await sender.Send(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/meal-plans/{id}", id),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.MealPlans);
    }

    internal sealed class Request
    {
        public Guid HouseholdId { get; init; }
        public string Name { get; init; } = string.Empty;
        public Guid CreatedBy { get; init; }

        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
    }
}
