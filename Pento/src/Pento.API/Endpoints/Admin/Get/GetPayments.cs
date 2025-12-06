using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
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
            GetAdminPaymentsSortBy? sortBy,
            SortOrder? sortOrder,
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
                sortBy,
                sortOrder ?? SortOrder.ASC,
                isDeleted,
                pageNumber,
                pageSize);
            Result<AdminPaymentsResponse> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Admin);
    }
}
