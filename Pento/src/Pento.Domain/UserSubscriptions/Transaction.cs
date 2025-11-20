using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pento.Domain.UserSubscriptions;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TransactionType { get; set; } = "TemplatePurchase";
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string? PaymentMethod { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Description { get; set; }
    public int? OrderCode { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public DateTime CreatedAt { get; set; } 
}





