using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Payments.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Payments.Get;

internal sealed class GetPaymentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("payments/{paymentId:guid}", async (
            Guid paymentId,
            IQueryHandler<GetPaymentByIdQuery, PaymentResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<PaymentResponse> result = await handler.Handle(new GetPaymentByIdQuery(paymentId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization().WithTags(Tags.Payments);
    }

}
