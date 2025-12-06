using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;

namespace Pento.Application.Payments.GetById;

internal sealed class GetPaymentByIdQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetPaymentByIdQuery, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql = $@"
            SELECT 
                Id AS {nameof(PaymentResponse.PaymentId)},
                order_code AS {nameof(PaymentResponse.OrderCode)},
                description AS {nameof(PaymentResponse.Description)},
                provider_description AS {nameof(PaymentResponse.ProviderDescription)},
                CONCAT(amount_due::text, ' ', currency) AS {nameof(PaymentResponse.AmountDue)},
                CONCAT(amount_paid::text, ' ', currency) AS {nameof(PaymentResponse.AmountPaid)},
                status AS {nameof(PaymentResponse.Status)},
                checkout_url AS {nameof(PaymentResponse.CheckoutUrl)},
                qr_code AS {nameof(PaymentResponse.QrCode)},
                created_at AS {nameof(PaymentResponse.CreatedAt)},
                expires_at AS {nameof(PaymentResponse.ExpiresAt)},
                paid_at AS {nameof(PaymentResponse.PaidAt)},
                cancelled_at AS {nameof(PaymentResponse.CancelledAt)},
                cancellation_reason AS {nameof(PaymentResponse.CancellationReason)}
            FROM Payments
            WHERE Id = @PaymentId AND is_deleted is false AND user_id = @UserId;
        ";
        CommandDefinition command = new(sql, new { request.PaymentId, userContext.UserId }, cancellationToken: cancellationToken);
        PaymentResponse? payment = await connection.QuerySingleOrDefaultAsync<PaymentResponse>(command);
        if (payment == null)
        {
            return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);
        }
        return Result.Success(payment);
    }
}
