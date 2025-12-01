using Pento.Application.Abstractions.Pagination;

namespace Pento.Application.Payments.GetAll;

public sealed record AdminPaymentsResponse(
    AdminPaymentSummary Summary,
    PagedList<AdminPaymentPreview> Payments
    );
public sealed record AdminPaymentSummary
{
    public string TotalAmount { get; init; }
    public int Pending { get; init; }
    public int Paid { get; init; }
    public int Failed { get; init; }
    public int Cancelled { get; init; }
    public int Expired { get; init; }

}
public sealed record AdminPaymentPreview
{
    public Guid PaymentId { get; init; }
    public Guid UserId { get; init; }
    public long OrderCode { get; init; }
    public string Description { get; init; }
    public string Amount { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
}
