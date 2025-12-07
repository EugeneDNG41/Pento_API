using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.External.PayOS;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;

namespace Pento.Application.Payments.Cancel;

internal sealed class CancelPaymentCommandHandler(
    IUserContext userContext,
    IGenericRepository<Payment> paymentRepository,
    IPayOSService service) : ICommandHandler<CancelPaymentCommand>
{
    public async Task<Result> Handle(CancelPaymentCommand command, CancellationToken cancellationToken)
    {
        Payment? payment = await paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);
        if (payment == null)
        {
            return Result.Failure(PaymentErrors.NotFound);
        }
        if (payment.UserId != userContext.UserId)
        {
            return Result.Failure(PaymentErrors.ForbiddenAccess);
        }
        Result result = await service.CancelPaymentAsync(payment, command.Reason, cancellationToken);
        return result;
    }
}
