using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Payments.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetPayments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/payments", async (
            Guid? userId,
            string? searchText,
            long? fromAmount,
            long? toAmount,
            DateTime? fromDate,
            DateTime? toDate,
            PaymentStatus? status,
            bool? isDeleted,
            IQueryHandler<GetAdminPaymentsQuery, AdminPaymentsResponse> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetAdminPaymentsQuery(
                userId,
                searchText,
                fromAmount,
                toAmount,
                fromDate?.ToUniversalTime(),
                toDate?.ToUniversalTime(),
                status,
                isDeleted,
                pageNumber,
                pageSize);
            Result<AdminPaymentsResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Admin);
    }
}
