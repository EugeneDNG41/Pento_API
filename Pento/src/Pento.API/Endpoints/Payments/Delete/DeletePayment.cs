using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.PayOS;
using Pento.Application.Payments.Cancel;
using Pento.Application.Payments.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Payments.Delete;

internal sealed class DeletePayment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("payments/{paymentId:guid}", async (
            Guid paymentId,
            ICommandHandler<DeletePaymentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new DeletePaymentCommand(paymentId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).RequireAuthorization()
        .WithTags(Tags.Payments);
    }
}
