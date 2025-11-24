using PayOS.Models.V2.PaymentRequests;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.PayOS;
using Pento.Application.Payments.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Payments.Post;

internal sealed class CreatePaymentLink : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("payments", async (
            Request request,
            ICommandHandler<CreatePaymentCommand, PaymentLinkResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<PaymentLinkResponse> result = await handler.Handle(
                new CreatePaymentCommand(request.Amount, request.Description, request.ReturnUrl, request.CancelUrl), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Payments);
    }
    internal sealed class Request
    {
        public long Amount { get; init; }
        public string Description { get; init; }
        public string ReturnUrl { get; init; }
        public string CancelUrl { get; init; }
    }
}
