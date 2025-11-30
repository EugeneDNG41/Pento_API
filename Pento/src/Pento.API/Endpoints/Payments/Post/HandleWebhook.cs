using PayOS.Models.Webhooks;
using Pento.API.Extensions;
using Pento.Application.Abstractions.ThirdPartyServices.PayOS;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Payments.Post;

internal sealed class HandleWebhook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("payments/webhook/handle", async (
            Webhook webhook,
            IPayOSService service,
            CancellationToken cancellationToken) =>
        {
            Result result = await service.HandleWebhookAsync(webhook, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Payments);
    }
}
