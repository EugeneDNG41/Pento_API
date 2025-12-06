using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.External.PayOS;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;

namespace Pento.Application.Payments.Delete;

internal sealed class DeletePaymentCommandHandler(
    IPayOSService service,
    IUserContext userContext, 
    IGenericRepository<Payment> paymentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeletePaymentCommand>
{
    public async Task<Result> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        Payment? payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
        {
            return Result.Failure(PaymentErrors.NotFound);
        }
        if (payment.UserId != userContext.UserId)
        {
            return Result.Failure(PaymentErrors.ForbiddenAccess);
        }
        if (payment.Status == PaymentStatus.Pending || payment.Status == PaymentStatus.Processing)
        {
            Result cancellationResult = await service.CancelPaymentAsync(payment, "Deleted by user", cancellationToken);
            if (cancellationResult.IsFailure)
            {
                return Result.Failure(PaymentErrors.PaymentCancellationFailed);
            }
        }
        payment.Delete();
        paymentRepository.Update(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
