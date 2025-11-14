
using Pento.API.Extensions;
using Pento.Application.Abstractions.Converter;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Utility.Post;

internal sealed class ConvertMeasurement : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("utility/convert-measurement", async (
            Request request,
            IConverterService converterService,
            CancellationToken cancellationToken) =>
        {
            Result<decimal> result = await converterService.ConvertAsync(
                request.Quantity,
                request.FromUnitId,
                request.ToUnitId,
                cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Utility)
        .RequireAuthorization()
        .WithDescription("Convert a measurement from one unit to another");
    }
    internal sealed class Request
    {
        public decimal Quantity { get; set; }
        public Guid FromUnitId { get; set; }
        public Guid ToUnitId { get; set; }
    }
}
