using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.PayOS;
using Pento.Application.Payments.Cancel;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Payments.Put;

internal sealed class CancelPayment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("payments/{paymentId:guid}/cancel", async (
            Guid paymentId,
            Request request,
            ICommandHandler<CancelPaymentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new CancelPaymentCommand(paymentId, request.Reason), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).RequireAuthorization()
        .WithTags(Tags.Payments);
    }
    internal sealed class Request
    {
        public string? Reason { get; init; }
    }
}
