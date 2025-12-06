using Pento.API.Extensions;
using Pento.Application.Abstractions.External.PayOS;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Payments.Post;

internal sealed class ConfirmWebhook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("payments/webhook/confirm", async (IPayOSService service) =>
        {
            Result result = await service.ConfirmWebhookAsync();
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).RequireAuthorization(Permissions.ManagePayments).WithTags(Tags.Payments);
    }
}
