using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.PayOS;
using Pento.Domain.Abstractions;

namespace Pento.Application.Payments.Cancel;

internal sealed class CancelPaymentCommandHandler(IPayOSService service) : ICommandHandler<CancelPaymentCommand>
{
    public async Task<Result> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
    {
        Result result = await service.CancelPaymentAsync(request.PaymentId, request.Reason, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);

        }
        return Result.Success();
    }
}
