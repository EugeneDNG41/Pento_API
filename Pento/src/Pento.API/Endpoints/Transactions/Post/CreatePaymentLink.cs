using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using Pento.API.Extensions;
using Pento.Application.Abstractions.PayOS;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Transactions.Post;

internal sealed class CreatePaymentLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("transactions/payment-link", async (
            Request request,
            IPayOSService service) =>
        {
            Result<CreatePaymentLinkResponse> result = await service.CreatePaymentAsync(request.Amount, request.Description);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Transactions);
    }
    internal sealed class Request
    {
        public long Amount { get; init; }
        public string Description { get; init; }
    }
}
internal sealed class ConfirmWebhook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("transactions/webhook/confirm", async (IPayOSService service) =>
        {
            Result result = await service.ConfirmWebhookAsync();
            return result.Match(() => Results.Ok(), CustomResults.Problem);
        });
    }
}

internal sealed class HandleWebhook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("transactions/webhook/handle", async (
            Webhook webhook,
            IPayOSService service) =>
        {
            Result result = await service.HandleWebhookAsync(webhook);
            return result.Match(() => Results.Ok(), CustomResults.Problem);
        });
    }
}
