using Pento.API.Extensions;
using Pento.Application.Abstractions.PayOS;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Payments.Post;

internal sealed class ConfirmWebhook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("payments/webhook/confirm", async (IPayOSService service) =>
        {
            Result result = await service.ConfirmWebhookAsync();
            return result.Match(() => Results.Ok(), CustomResults.Problem);
        }).WithTags(Tags.Payments);
    }
}
