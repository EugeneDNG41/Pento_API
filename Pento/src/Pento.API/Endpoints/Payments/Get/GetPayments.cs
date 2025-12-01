using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Payments.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;

namespace Pento.API.Endpoints.Payments.Get;

internal sealed class GetPayments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("payments", async (
            string? searchText,
            long? fromAmount,
            long? toAmount,
            DateTime? fromDate,
            DateTime? toDate,
            PaymentStatus? status,            
            IQueryHandler<GetPaymentsQuery, PagedList<PaymentPreview>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetPaymentsQuery(
                searchText,
                fromAmount,
                toAmount,
                fromDate?.ToUniversalTime(),
                toDate?.ToUniversalTime(),
                status,
                pageNumber,
                pageSize);
            Result<PagedList<PaymentPreview>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization().WithTags(Tags.Payments);
    }
}
